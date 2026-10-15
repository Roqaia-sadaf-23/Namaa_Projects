using MediatR;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Queries.GetServices;

public sealed record GetServicesQuery : IRequest<Result<IReadOnlyList<ServiceDto>>>;

