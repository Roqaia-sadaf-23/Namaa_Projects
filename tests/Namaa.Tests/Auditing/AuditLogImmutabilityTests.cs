using System.Reflection;
using Namaa.Domain.Entities;

namespace Namaa.Tests.Auditing;

public sealed class AuditLogImmutabilityTests
{
    [Fact]
    public void AuditLogHasNoPublicSettersOrMutationMethods()
    {
        var publicSetters = typeof(AuditLog)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.SetMethod?.IsPublic == true);
        var publicMutationMethods = typeof(AuditLog)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName);

        Assert.Empty(publicSetters);
        Assert.Empty(publicMutationMethods);
    }
}
