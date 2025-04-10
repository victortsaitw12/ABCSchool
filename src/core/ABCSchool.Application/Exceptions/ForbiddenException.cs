using System;
using System.Net;
namespace ABCSchool.Application.Exceptions;

public class ForbiddenException: Exception
{
    public List<string> ErrorMessages { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public ForbiddenException(
        List<string> errorMessages = default,
        HttpStatusCode statusCode = HttpStatusCode.Forbidden
    )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
