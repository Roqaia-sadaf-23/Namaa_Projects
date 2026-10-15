using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Queries.GetServices;

public sealed class GetServicesQueryHandler(IServiceRepository repository) : IRequestHandler<GetServicesQuery, Result<IReadOnlyList<ServiceDto>>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<ServiceDto>>(entities.Select(ServiceDto.FromEntity).ToList());
    }
}

