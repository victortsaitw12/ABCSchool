using System;

namespace ABCSchool.Application.Features.Identity.Roles;

public class CreateRoleRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
}
