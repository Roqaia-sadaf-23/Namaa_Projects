using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Application.Common.Concurrency;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Enums;

namespace Namaa.Application.Projects.Commands.ChangeProjectStatus;

public sealed record ChangeProjectStatusCommand(long ProjectId, ProjectStatus Status, string? Reason, string RowVersion) : IRequest<Result<ProjectDto>>, IConcurrencyCommand;

