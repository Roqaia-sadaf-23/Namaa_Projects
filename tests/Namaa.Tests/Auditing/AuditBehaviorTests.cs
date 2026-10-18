using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Auditing;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Behaviors;
using Namaa.Application.Common.Auditing;
using Namaa.Application.ProjectMembers.Commands.AddProjectMember;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Application.Projects.Commands.CreateProject;
using Namaa.Application.Projects.Commands.UpdateProject;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Enums;

namespace Namaa.Tests.Auditing;

public sealed class AuditBehaviorTests
{
    [Fact]
    public async Task SuccessfulMutationCreatesOneAuditRecordWithStableNames()
    {
        var command = CreateValidProjectCommand();
        var auditTrail = new RecordingAuditTrail();
        var behavior = new AuditBehavior<CreateProjectCommand, Result<ProjectDto>>(
            auditTrail,
            new TestCurrentUser(25));

        var response = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(CreateProjectDto(101))),
            CancellationToken.None);

        Assert.True(response.IsSuccess);
        var record = Assert.Single(auditTrail.Records);
        Assert.Equal(25, record.ActorUserId);
        Assert.Equal("Project.Created", record.Action);
        Assert.Equal("Project", record.EntityType);
        Assert.Equal(101, record.EntityId);
    }

    [Fact]
    public async Task FailedBusinessResultCreatesNoAuditRecord()
    {
        var command = CreateValidProjectCommand();
        var auditTrail = new RecordingAuditTrail();
        var behavior = new AuditBehavior<CreateProjectCommand, Result<ProjectDto>>(
            auditTrail,
            new TestCurrentUser(25));
        var failure = new Error("Projects.NotFound", "The project was not found.", ErrorType.NotFound);

        var response = await behavior.Handle(
            command,
            () => Task.FromResult(Result.Failure<ProjectDto>(failure)),
            CancellationToken.None);

        Assert.True(response.IsFailure);
        Assert.Empty(auditTrail.Records);
    }

    [Fact]
    public async Task FailedValidationNeverStartsAuditTransaction()
    {
        var command = CreateValidProjectCommand() with { CustomerId = 0 };
        var auditTrail = new RecordingAuditTrail();
        var auditBehavior = new AuditBehavior<CreateProjectCommand, Result<ProjectDto>>(
            auditTrail,
            new TestCurrentUser(25));
        var validationBehavior = new ValidationBehavior<CreateProjectCommand, Result<ProjectDto>>(
            new[] { new CreateProjectCommandValidator() });
        var handlerReached = false;

        await Assert.ThrowsAsync<ValidationException>(() => validationBehavior.Handle(
            command,
            () => auditBehavior.Handle(
                command,
                () =>
                {
                    handlerReached = true;
                    return Task.FromResult(Result.Success(CreateProjectDto(101)));
                },
                CancellationToken.None),
            CancellationToken.None));

        Assert.False(handlerReached);
        Assert.Equal(0, auditTrail.ExecuteCount);
        Assert.Empty(auditTrail.Records);
    }

    [Fact]
    public async Task StaleConcurrencyFailureCreatesNoSuccessAuditRecord()
    {
        var command = new UpdateProjectCommand(
            101,
            "Project",
            1000,
            null,
            null,
            null,
            null,
            null,
            Convert.ToBase64String(new byte[8]));
        var auditTrail = new RecordingAuditTrail();
        var behavior = new AuditBehavior<UpdateProjectCommand, Result<ProjectDto>>(
            auditTrail,
            new TestCurrentUser(25));

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => behavior.Handle(
            command,
            () => throw new DbUpdateConcurrencyException("Stale rowversion."),
            CancellationToken.None));

        Assert.Empty(auditTrail.Records);
    }

    [Fact]
    public async Task BusinessTargetUserCannotSpoofAuditActor()
    {
        var command = new AddProjectMemberCommand(101, 999, DateTime.UtcNow, "Engineer");
        var auditTrail = new RecordingAuditTrail();
        var behavior = new AuditBehavior<AddProjectMemberCommand, Result<ProjectMemberDto>>(
            auditTrail,
            new TestCurrentUser(25));
        var dto = new ProjectMemberDto(55, 101, 999, "Engineer", command.JoinedAt, null);

        await behavior.Handle(
            command,
            () => Task.FromResult(Result.Success(dto)),
            CancellationToken.None);

        var record = Assert.Single(auditTrail.Records);
        Assert.Equal(25, record.ActorUserId);
        Assert.NotEqual(command.UserId, record.ActorUserId);
        Assert.Equal("ProjectMember.Added", record.Action);
        Assert.Equal(55, record.EntityId);
    }

    private static CreateProjectCommand CreateValidProjectCommand() =>
        new(1, "PRJ-001", "Project", 1000, null, null, null, null, null, null);

    private static ProjectDto CreateProjectDto(long id) =>
        new(
            id,
            1,
            null,
            null,
            "PRJ-001",
            "Project",
            null,
            null,
            null,
            null,
            null,
            ProjectStatus.Planned,
            0,
            1000,
            25,
            DateTime.UtcNow,
            null,
            Convert.ToBase64String(new byte[8]));

    private sealed class RecordingAuditTrail : IAuditTrail
    {
        public List<AuditRecord> Records { get; } = [];
        public int ExecuteCount { get; private set; }

        public async Task<TResponse> ExecuteAsync<TResponse>(
            Func<Task<TResponse>> operation,
            Func<TResponse, AuditRecord?> auditRecordFactory,
            CancellationToken cancellationToken = default)
        {
            ExecuteCount++;
            var response = await operation();
            var record = auditRecordFactory(response);

            if (record is not null)
            {
                Records.Add(record);
            }

            return response;
        }
    }

    private sealed class TestCurrentUser(long? userId) : ICurrentUser
    {
        public long? UserId { get; } = userId;
        public bool IsAuthenticated => UserId.HasValue;
        public IReadOnlyCollection<string> Roles { get; } = Array.Empty<string>();
    }
}
