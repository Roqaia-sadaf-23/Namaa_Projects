using MediatR;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondenceById;

public sealed record GetProjectCorrespondenceByIdQuery(long ProjectId, long Id) : IRequest<Result<ProjectCorrespondenceDto>>;

