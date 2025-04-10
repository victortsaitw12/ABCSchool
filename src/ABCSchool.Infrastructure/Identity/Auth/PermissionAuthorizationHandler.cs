using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ABCSchool.Infrastructure.Constants;

namespace ABCSchool.Infrastructure.Identity.Auth;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirment>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirment requirement)
    {
        var permissions = context.User.Claims
            .Where(x => x.Type == ClaimConstants.Permission)
            .Select(x => x.Value)
            .ToList();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
