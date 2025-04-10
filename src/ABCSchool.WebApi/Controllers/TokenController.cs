using ABCSchool.Application.Features.Identity.Tokens;
using ABCSchool.Application.Features.Identity.Tokens.Queries;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Identity.Auth;
using ABCSchool.Infrastructure.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;


namespace ABCSchool.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : BaseApiController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        [TenantHeader]
        [OpenApiOperation("Used to obtain a token for login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var response = await Sender.Send(new GetTokenQuery{
                TokenRequest = tokenRequest
            });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }        


        [HttpPost("refresh-token")]
        [OpenApiOperation("Used to generate new jwt from refresh token")]
        [ShouldHavePermission(action: SchoolAction.RefreshToken, feature: SchoolFeature.Tokens)]
        public async Task<IActionResult> GetRefreshTokenAsync(
            [FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await Sender.Send(new GetRefreshTokenQuery{
                RefreshToken = refreshTokenRequest
            });
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
