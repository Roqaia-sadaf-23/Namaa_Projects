using MediatR;
using Namaa.Application.ProjectStatusHistory.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistoryById;

public sealed record GetProjectStatusHistoryByIdQuery(long ProjectId, long Id) : IRequest<Result<ProjectStatusHistoryDto>>;

