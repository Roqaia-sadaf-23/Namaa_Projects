namespace Namaa.Api.Authorization;

public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddNamaaAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageProjects);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageCrmSales);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageFinance);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanPostJournalEntries);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageUsers);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageRoles);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanManageProcurement);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanViewNotifications);
            AddAuthenticatedPolicy(options, AuthorizationPolicies.CanViewAuditLogs);
        });

        return services;
    }

    private static void AddAuthenticatedPolicy(
        Microsoft.AspNetCore.Authorization.AuthorizationOptions options,
        string policyName) =>
        options.AddPolicy(policyName, policy => policy.RequireAuthenticatedUser());
}
