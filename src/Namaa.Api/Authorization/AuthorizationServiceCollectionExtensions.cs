namespace Namaa.Api.Authorization;

public static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddNamaaAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var policyName in AuthorizationPolicyRoleMap.PolicyNames)
            {
                var allowedRoles = AuthorizationPolicyRoleMap.GetAllowedRoles(policyName);
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    if (allowedRoles.Length > 0)
                        policy.RequireRole(allowedRoles);
                });
            }
        });

        return services;
    }
}
