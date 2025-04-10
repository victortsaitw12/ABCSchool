using ABCSchool.Application.Features.Schools;
using ABCSchool.Application.Features.Schools.Commands;
using ABCSchool.Application.Features.Schools.Queries;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCSchool.WebApi.Controllers
{
    [Route("api/[controller]")]
 
    public class SchoolsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Schools)]
        public async Task<IActionResult> CreateSchoolAsync([FromBody] CreateSchoolRequest createSchool)
        {
            var response = await Sender.Send(new CreateSchoolCommand {CreateSchool = createSchool});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("update")]
        [ShouldHavePermission(SchoolAction.Update, SchoolFeature.Schools)]
        public async Task<IActionResult> UpdateSchoolAsync([FromBody] UpdateSchoolRequest updateSchool)
        {
            var response = await Sender.Send(new UpdateSchoolCommand {UpdateSchool = updateSchool});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }


        [HttpDelete("{schoolId}")]
        [ShouldHavePermission(SchoolAction.Delete, SchoolFeature.Schools)]
        public async Task<IActionResult> DeleteSchoolAsync(int schoolId)
        {
            var response = await Sender.Send(new DeleteSchoolCommand { schoolId = schoolId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        
        [HttpGet("by-id/{schoolId}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolByIdAsync(int schoolId)
        {
            var response = await Sender.Send(new GetSchoolByIdQuery { SchoolId = schoolId});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("by-name/{name}")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolByNameAsync(string name)
        {
            var response = await Sender.Send(new GetSchoolByNameQuery { Name = name});
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

      
        [HttpGet("all")]
        [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetAllSchoolsAsync()
        {
            var response = await Sender.Send(new GetSchoolsQuery());
            if(response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);

        }
    }

}
