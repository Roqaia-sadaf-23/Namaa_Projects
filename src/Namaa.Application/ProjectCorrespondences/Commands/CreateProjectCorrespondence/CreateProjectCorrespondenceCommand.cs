using MediatR;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Commands.CreateProjectCorrespondence;

public sealed record CreateProjectCorrespondenceCommand(long ProjectId, string CorrespondenceType, string Subject, DateTime CorrespondenceDate, string? ReferenceNumber, string? Notes) : IRequest<Result<ProjectCorrespondenceDto>>;

