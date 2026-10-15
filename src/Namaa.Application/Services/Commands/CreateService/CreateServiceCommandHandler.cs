using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Services.Commands.CreateService;

public sealed class CreateServiceCommandHandler(IServiceRepository repository, IApplicationDbContext dbContext) : IRequestHandler<CreateServiceCommand, Result<ServiceDto>>
{
    public async Task<Result<ServiceDto>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = Service.Create(request.Name, request.DefaultPrice, request.Category, request.Description);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ServiceDto.FromEntity(entity));
    }
}

