using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application.Abstractions.Behaviors;

public sealed class ConcurrencyBehavior<TRequest, TResponse>(
    IConcurrencyTokenContext concurrencyTokenContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IConcurrencyCommand concurrencyCommand)
        {
            return await next();
        }

        if (!ConcurrencyToken.TryDecode(concurrencyCommand.RowVersion, out var rowVersion))
        {
            throw new ValidationException(
                new[]
                {
                    new ValidationFailure(
                        nameof(IConcurrencyCommand.RowVersion),
                        "RowVersion is required and must be a valid, non-empty Base64 value.")
                });
        }

        concurrencyTokenContext.UseExpectedRowVersion(rowVersion);

        try
        {
            return await next();
        }
        finally
        {
            concurrencyTokenContext.ClearExpectedRowVersion();
        }
    }
}
