using System;
using Microsoft.AspNetCore.Identity;

namespace ABCSchool.Infrastructure.Identity;

public class ApplicationRoleClaim: IdentityRoleClaim<string>
{
    public string Description { get; set; }
    public string Group { get; set; }
}
