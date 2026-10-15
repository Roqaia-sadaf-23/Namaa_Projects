using MediatR;
using Namaa.Application.ProjectStatusHistory.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistory;

public sealed record GetProjectStatusHistoryQuery(long ProjectId) : IRequest<Result<IReadOnlyList<ProjectStatusHistoryDto>>>;

