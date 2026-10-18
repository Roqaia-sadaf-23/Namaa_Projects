using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Namaa.Api.Authorization;
using Namaa.Application.Common.Authorization;

namespace Namaa.Tests.Authorization;

public sealed class AuthorizationPolicyTests
{
    [Fact]
    public async Task CapabilityPolicies_ContainTheDocumentedRoleSets()
    {
        var expectedRoles = new Dictionary<string, string[]>
        {
            [AuthorizationPolicies.CanManageProjects] =
                [RoleNames.Administrator, RoleNames.ProjectManager],
            [AuthorizationPolicies.CanManageCrmSales] =
                [RoleNames.Administrator, RoleNames.Sales],
            [AuthorizationPolicies.CanManageFinance] =
                [RoleNames.Administrator, RoleNames.Accountant],
            [AuthorizationPolicies.CanPostJournalEntries] =
                [RoleNames.Administrator, RoleNames.Accountant],
            [AuthorizationPolicies.CanManageUsers] =
                [RoleNames.Administrator, RoleNames.HR],
            [AuthorizationPolicies.CanManageRoles] =
                [RoleNames.Administrator],
            [AuthorizationPolicies.CanManageProcurement] =
                [RoleNames.Administrator, RoleNames.Procurement],
            [AuthorizationPolicies.CanViewNotifications] = [],
            [AuthorizationPolicies.CanViewAuditLogs] =
                [RoleNames.Administrator]
        };
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddNamaaAuthorization();
        await using var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();

        foreach (var pair in expectedRoles)
        {
            var policy = await policyProvider.GetPolicyAsync(pair.Key);

            Assert.NotNull(policy);
            Assert.Contains(policy.Requirements, requirement =>
                requirement is DenyAnonymousAuthorizationRequirement);
            var configuredRoles = policy.Requirements
                .OfType<RolesAuthorizationRequirement>()
                .SelectMany(requirement => requirement.AllowedRoles)
                .OrderBy(role => role, StringComparer.Ordinal)
                .ToArray();
            Assert.Equal(
                pair.Value.OrderBy(role => role, StringComparer.Ordinal),
                configuredRoles);
        }
    }

    [Fact]
    public void RoleCatalog_IsMinimalUniqueAndWithinSchemaLimit()
    {
        Assert.Equal(7, RoleNames.All.Count);
        Assert.Equal(RoleNames.All.Count, RoleNames.All.Distinct(StringComparer.Ordinal).Count());
        Assert.All(RoleNames.All, roleName =>
        {
            Assert.False(string.IsNullOrWhiteSpace(roleName));
            Assert.True(roleName.Length <= 30);
        });
    }
}
