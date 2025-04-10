using System;

namespace ABCSchool.Application.Features.Identity.Tokens;

public interface ITokenService
{
    Task<TokenResponse> LoginAsync(TokenRequest request);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
}
