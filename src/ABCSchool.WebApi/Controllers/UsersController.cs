using ABCSchool.Application.Features.Identity.Users;
using ABCSchool.Application.Features.Identity.Users.Commands;
using ABCSchool.Application.Features.Identity.Users.Queries;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCSchool.WebApi.Controllers
{
    [Route("api/[controller]")]
   
    public class UsersController : BaseApiController
    {
        [HttpPost("register")]
        [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Users)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] CreateUserRequest createUser)
        {
            var response = await Sender.Send(new CreateUserCommand{ CreateUser = createUser});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
        public async Task<IActionResult> UpdateUserDetailsAsync([FromBody] UpdateUserRequest updateUser)
        {
            var response = await Sender.Send(new UpdateUserCommand{ UpdateUser = updateUser});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-status")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
        public async Task<IActionResult> ChangeUserStatusAsync([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await Sender.Send(new UpdateUserStatusCommand{ ChangeUserStatus = changeUserStatus});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update-roles/{roleId}")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Users)]
        public async Task<IActionResult> UpdateUserRolesAsync(string roleId, [FromBody] UserRolesRequest userRolesRequest)
        {
            var response = await Sender.Send(new UpdateUserRolesCommand{ RoleId = roleId, userRolesRequest = userRolesRequest});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete/{userId}")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Users)]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var response = await Sender.Send(new DeleteUserCommand{ UserId = userId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await Sender.Send(new GetAllUsersQuery());
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        
        [HttpGet("{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            var response = await Sender.Send(new GetUserByIdQuery{ UserId = userId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("permissions/{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.RoleClaims)]
        public async Task<IActionResult> GetUserPermissionsAsync(string userId)
        {
            var response = await Sender.Send(new GetUserPermissionsQuery{ UserId = userId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("user-roles/{userId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Users)]
        public async Task<IActionResult> GetUserRolesAsync(string userId)
        {
            var response = await Sender.Send(new GetUserRolesQuery{ UserId = userId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        
        
    }


}
