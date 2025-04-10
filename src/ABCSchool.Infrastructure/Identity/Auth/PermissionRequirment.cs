using System;
using Microsoft.AspNetCore.Authorization;
namespace ABCSchool.Infrastructure.Identity.Auth;

public class PermissionRequirment: IAuthorizationRequirement
{
    public string Permission { get; set; }

    public PermissionRequirment(string permission)
    {
        Permission = permission;
    }
}

