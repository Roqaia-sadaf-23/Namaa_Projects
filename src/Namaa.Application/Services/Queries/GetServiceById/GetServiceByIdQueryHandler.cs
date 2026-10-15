using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Queries.GetServiceById;

public sealed class GetServiceByIdQueryHandler(IServiceRepository repository) : IRequestHandler<GetServiceByIdQuery, Result<ServiceDto>>
{
    public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindByIdAsync(request.Id, cancellationToken);
        return entity is null ? Result.Failure<ServiceDto>(ServiceErrors.NotFound) : Result.Success(ServiceDto.FromEntity(entity));
    }
}

