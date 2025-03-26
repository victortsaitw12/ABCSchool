using System;
using Microsoft.AspNetCore.Identity;

namespace ABCSchool.Infrastructure.Identity;

public class ApplicationRole: IdentityRole
{
    public string Description { get; set; }
}
