using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Namaa.Infrastructure;

public static class DependencyInjection
{
    // SQL Server registrations belong here, but await the real schema to avoid inventing a database model.
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services;
    }
}
