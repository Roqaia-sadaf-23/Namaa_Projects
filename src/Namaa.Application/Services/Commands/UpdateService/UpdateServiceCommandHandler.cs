using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Services.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Commands.UpdateService;

public sealed class UpdateServiceCommandHandler(IServiceRepository repository, IApplicationDbContext dbContext) : IRequestHandler<UpdateServiceCommand, Result<ServiceDto>>
{
    public async Task<Result<ServiceDto>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindForUpdateAsync(request.Id, cancellationToken);
        if (entity is null) return Result.Failure<ServiceDto>(ServiceErrors.NotFound);
        entity.Update(request.Name, request.DefaultPrice, request.Category, request.Description);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ServiceDto.FromEntity(entity));
    }
}

