using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABCSchool.WebApi.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private ISender _sender;

        public ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
