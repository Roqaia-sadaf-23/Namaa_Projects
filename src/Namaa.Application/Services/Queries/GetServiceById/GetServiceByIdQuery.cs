using MediatR;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Queries.GetServiceById;

public sealed record GetServiceByIdQuery(long Id) : IRequest<Result<ServiceDto>>;

