using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectFiles.Commands.UploadProjectFile;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Tests.Infrastructure;

public sealed class ProjectFileUploadCompensationTests
{
    [Fact]
    public async Task Upload_WhenMetadataSaveFails_DeletesStoredFile()
    {
        var storage = new TrackingFileStorage();
        var handler = new UploadProjectFileCommandHandler(
            new RecordingProjectFileRepository(),
            new ExistingProjectRepository(),
            new ExistingUserRepository(),
            storage,
            new AllowingUploadPolicy(),
            new AuthenticatedUser(),
            new ThrowingApplicationDbContext(),
            NullLogger<UploadProjectFileCommandHandler>.Instance);
        await using var content = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(
            new UploadProjectFileCommand(
                10,
                content,
                "document.txt",
                "text/plain",
                3,
                null),
            CancellationToken.None));

        Assert.Equal("objects/aa/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", storage.DeletedStorageKey);
    }

    private sealed class TrackingFileStorage : IFileStorage
    {
        public string? DeletedStorageKey { get; private set; }

        public Task<StoredFileResult> SaveAsync(
            Stream content,
            string fileName,
            string? contentType,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new StoredFileResult(
                "objects/aa/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                3,
                contentType));

        public Task<Stream?> OpenReadAsync(
            string storageKey,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Stream?>(null);

        public Task<bool> ExistsAsync(
            string storageKey,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            DeletedStorageKey = storageKey;
            return Task.CompletedTask;
        }
    }

    private sealed class AllowingUploadPolicy : IFileUploadPolicy
    {
        public Result<ValidatedFileUpload> Validate(
            string fileName,
            string? contentType,
            long declaredSizeBytes) =>
            Result.Success(new ValidatedFileUpload(fileName, contentType));
    }

    private sealed class RecordingProjectFileRepository : IProjectFileRepository
    {
        public Task<bool> ExistsByStorageKeyAsync(
            string storageKey,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<ProjectFile?> FindByIdAsync(
            long projectId,
            long id,
            CancellationToken cancellationToken = default) => Task.FromResult<ProjectFile?>(null);

        public Task<ProjectFile?> FindForUpdateAsync(
            long projectId,
            long id,
            CancellationToken cancellationToken = default) => Task.FromResult<ProjectFile?>(null);

        public Task<IReadOnlyList<ProjectFile>> GetByProjectIdAsync(
            long projectId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProjectFile>>([]);

        public Task AddAsync(
            ProjectFile file,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class ExistingProjectRepository : IProjectRepository
    {
        private readonly Project _project = Project.Create(1, "P", "Project", 0, 1);

        public Task<bool> ExistsByCodeAsync(
            string projectCode,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> ExistsBySourceQuotationIdAsync(
            long sourceQuotationId,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<Project?> FindByIdAsync(
            long id,
            CancellationToken cancellationToken = default) => Task.FromResult<Project?>(_project);

        public Task<Project?> FindForUpdateAsync(
            long id,
            CancellationToken cancellationToken = default) => Task.FromResult<Project?>(null);

        public Task<IReadOnlyList<Project>> GetAllAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Project>>([_project]);

        public Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class ExistingUserRepository : IUserRepository
    {
        public Task<bool> ExistsByIdAsync(
            long id,
            CancellationToken cancellationToken = default) => Task.FromResult(true);

        public Task<User?> FindByIdAsync(long id, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(null);

        public Task<User?> FindByEmailWithRoleAsync(
            string email,
            CancellationToken cancellationToken = default) => Task.FromResult<User?>(null);

        public Task<User?> FindForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(null);

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<User>>([]);

        public Task<bool> EmailExistsAsync(
            string email,
            long excludingId,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> PhoneExistsAsync(
            string phoneNumber,
            long excludingId,
            CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class AuthenticatedUser : ICurrentUser
    {
        public long? UserId => 7;
        public bool IsAuthenticated => true;
        public IReadOnlyCollection<string> Roles => [];
    }

    private sealed class ThrowingApplicationDbContext : IApplicationDbContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            throw new DbUpdateException("Simulated metadata persistence failure.");
    }
}
