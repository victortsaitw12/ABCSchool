using System;
using System.Net;
namespace ABCSchool.Application.Exceptions;

public class NotFoundException: Exception
{
   public List<string> ErrorMessages { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public NotFoundException(
        List<string> errorMessages = default,
        HttpStatusCode statusCode = HttpStatusCode.NotFound
    )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
