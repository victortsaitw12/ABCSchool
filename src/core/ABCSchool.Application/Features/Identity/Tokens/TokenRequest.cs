using System;

namespace ABCSchool.Application.Features.Identity.Tokens;

public class TokenRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
