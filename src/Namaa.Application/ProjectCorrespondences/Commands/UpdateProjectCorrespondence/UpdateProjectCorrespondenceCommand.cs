using MediatR;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Commands.UpdateProjectCorrespondence;

public sealed record UpdateProjectCorrespondenceCommand(long ProjectId, long Id, string CorrespondenceType, string Subject, DateTime CorrespondenceDate, string? ReferenceNumber, string? Notes) : IRequest<Result<ProjectCorrespondenceDto>>;

