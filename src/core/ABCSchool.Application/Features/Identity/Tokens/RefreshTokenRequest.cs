using System;

namespace ABCSchool.Application.Features.Identity.Tokens;

public class RefreshTokenRequest
{
    public string CurrentJwt { get; set; }
    public string CurrentRefreshToken { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; }
}
