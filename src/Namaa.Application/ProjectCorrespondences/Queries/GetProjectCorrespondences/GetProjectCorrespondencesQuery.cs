using MediatR;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondences;

public sealed record GetProjectCorrespondencesQuery(long ProjectId) : IRequest<Result<IReadOnlyList<ProjectCorrespondenceDto>>>;

