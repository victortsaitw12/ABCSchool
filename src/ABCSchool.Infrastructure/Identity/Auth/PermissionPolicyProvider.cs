using System;
using ABCSchool.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ABCSchool.Infrastructure.Identity.Auth;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() 
    { 
        return _fallbackPolicyProvider.GetDefaultPolicyAsync(); 
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy>(null);
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string permission)
    {
       if(permission.StartsWith(ClaimConstants.Permission,StringComparison.OrdinalIgnoreCase))
       {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirment(permission));
            return Task.FromResult(policy.Build());
       }
       return _fallbackPolicyProvider.GetPolicyAsync(permission);
    }
}
