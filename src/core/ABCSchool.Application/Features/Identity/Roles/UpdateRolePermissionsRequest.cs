using System;

namespace ABCSchool.Application.Features.Identity.Roles;

public class UpdateRolePermissionsRequest
{
    public string RoleId { get; set; }
    public List<string> NewPermissions { get; set; } = [];
}
