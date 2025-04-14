using System;

namespace ABCSchool.Application.Features.Identity.Users;

public class UserResponse
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}
