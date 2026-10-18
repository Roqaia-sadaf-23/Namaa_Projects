using System.Globalization;
using MediatR;
using Namaa.Application.Abstractions.Auditing;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Common.Auditing;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Abstractions.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>(
    IAuditTrail auditTrail,
    ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!AuditCommandCatalog.TryGet(request.GetType(), out var definition))
        {
            return next();
        }

        return auditTrail.ExecuteAsync(
            () => next(),
            response => CreateAuditRecord(request, response, definition),
            cancellationToken);
    }

    private AuditRecord? CreateAuditRecord(
        TRequest request,
        TResponse response,
        AuditDefinition definition)
    {
        if (response is not IResult result)
        {
            throw new InvalidOperationException(
                $"Auditable command '{request.GetType().Name}' must return Result or Result<T>.");
        }

        if (result.IsFailure)
        {
            return null;
        }

        return new AuditRecord(
            currentUser.UserId,
            definition.Action,
            definition.EntityType,
            GetEntityId(response),
            definition.NewValuesFactory(request));
    }

    private static long GetEntityId(TResponse response)
    {
        var valueProperty = response!.GetType().GetProperty("Value")
            ?? throw new InvalidOperationException(
                "A successful auditable command must return a value containing an Id property.");
        var value = valueProperty.GetValue(response)
            ?? throw new InvalidOperationException(
                "A successful auditable command returned a null value.");
        var idProperty = value.GetType().GetProperty("Id")
            ?? throw new InvalidOperationException(
                $"Auditable response value '{value.GetType().Name}' does not expose an Id property.");
        var idValue = idProperty.GetValue(value)
            ?? throw new InvalidOperationException("An auditable response returned a null entity Id.");
        var entityId = Convert.ToInt64(idValue, CultureInfo.InvariantCulture);

        if (entityId <= 0)
        {
            throw new InvalidOperationException(
                $"An auditable response returned invalid entity Id '{entityId}'.");
        }

        return entityId;
    }
}
