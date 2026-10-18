using MediatR;
using Namaa.Application;
using Namaa.Application.Common.Auditing;

namespace Namaa.Tests.Auditing;

public sealed class AuditCommandCatalogTests
{
    [Fact]
    public void EveryBusinessMutationCommandHasOneValidAuditDefinition()
    {
        var commandTypes = typeof(ApplicationAssemblyMarker).Assembly.DefinedTypes
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.Namespace?.Contains(".Commands.", StringComparison.Ordinal) == true)
            .Where(type => type.Namespace?.Contains(".Authentication.", StringComparison.Ordinal) != true)
            .Where(type => type.ImplementedInterfaces.Any(interfaceType =>
                interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeof(IRequest<>)))
            .ToArray();

        Assert.NotEmpty(commandTypes);

        foreach (var commandType in commandTypes)
        {
            Assert.True(
                AuditCommandCatalog.TryGet(commandType, out var definition),
                $"{commandType.FullName} is missing from the audit command catalog.");
            Assert.StartsWith(
                definition.EntityType + ".",
                definition.Action,
                StringComparison.Ordinal);
            Assert.InRange(definition.Action.Length, 1, 50);
            Assert.InRange(definition.EntityType.Length, 1, 50);
        }
    }

    [Fact]
    public void AuthenticationCommandsAreNotBusinessAudited()
    {
        var authenticationCommands = typeof(ApplicationAssemblyMarker).Assembly.DefinedTypes
            .Where(type => type.Namespace?.Contains(".Authentication.Commands.", StringComparison.Ordinal) == true)
            .ToArray();

        Assert.NotEmpty(authenticationCommands);
        Assert.All(
            authenticationCommands,
            commandType => Assert.False(AuditCommandCatalog.TryGet(commandType, out _)));
    }
}
