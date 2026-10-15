using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Namaa.Application.Abstractions.Behaviors;

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

        return services;
    }
}

public sealed class ApplicationAssemblyMarker;
