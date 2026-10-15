using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Namaa.Application.Abstractions.Behaviors;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application;

public static class DependencyInjection
{
    // This is the composition boundary for use cases, validators, and MediatR handlers.
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>();
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(ConcurrencyBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        var concurrencyCommandTypes = typeof(ApplicationAssemblyMarker).Assembly
            .DefinedTypes
            .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IConcurrencyCommand).IsAssignableFrom(type));

        foreach (var commandType in concurrencyCommandTypes)
        {
            services.AddTransient(
                typeof(IValidator<>).MakeGenericType(commandType),
                typeof(ConcurrencyCommandValidator<>).MakeGenericType(commandType));
        }

        return services;
    }
}

public sealed class ApplicationAssemblyMarker;
