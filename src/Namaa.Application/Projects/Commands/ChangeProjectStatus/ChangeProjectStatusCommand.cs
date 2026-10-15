using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Enums;

namespace Namaa.Application.Projects.Commands.ChangeProjectStatus;

public sealed record ChangeProjectStatusCommand(long ProjectId, ProjectStatus Status, long ChangedByUserId, string? Reason) : IRequest<Result<ProjectDto>>;

