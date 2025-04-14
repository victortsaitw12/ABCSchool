using System;
using System.Security.Claims;

namespace ABCSchool.Application.Features.Identity.Users;

public interface ICurrentUserService
{
    string Name { get; }
    string GetUserId();
    string GetUserEmail();
    string GetUserTenant();
    bool IsAuthenticated();
    bool IsInRole(string roleName);
    IEnumerable<Claim> GetUserClaims();
    void SetCurrentUser(ClaimsPrincipal principal);
}
