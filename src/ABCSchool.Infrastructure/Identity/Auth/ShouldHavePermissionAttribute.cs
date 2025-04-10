using System;
using ABCSchool.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ABCSchool.Infrastructure.Identity.Auth;

public class ShouldHavePermissionAttribute: AuthorizeAttribute
{
    public ShouldHavePermissionAttribute(string action,string feature)
    {
        Policy = SchoolPermission.NameFor(action, feature);
    }
}
