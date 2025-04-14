using System;
using System.Security.Claims;
using ABCSchool.Application.Exceptions;
using ABCSchool.Application.Features.Identity.Users;

namespace ABCSchool.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal _principal;
    public string Name => _principal.Identity.Name;

    public IEnumerable<Claim> GetUserClaims()
    {
        return _principal.Claims;
    }

    public string GetUserEmail()
    {
        if(IsAuthenticated())
        {
            return _principal.GetEmail();
        }
        return string.Empty;
    }

    public string GetUserId()
    {
        if(IsAuthenticated())
        {
            return _principal.GetUserId();
        }
        return string.Empty;
    }    

    public string GetUserTenant()
    {
        if(IsAuthenticated())
        {
            return _principal.GetTenant();
        }
        return string.Empty;
    }

    public bool IsAuthenticated()
    {
        return _principal.Identity.IsAuthenticated;
    }

    public bool IsInRole(string roleName)
    {
        return _principal.IsInRole(roleName);
    }

    public void SetCurrentUser(ClaimsPrincipal principal)
    {
        if(_principal is not null)
        {
            throw new ConflictException(["Invalid operation on claim."]);
        }
        _principal = principal;
    }
}
