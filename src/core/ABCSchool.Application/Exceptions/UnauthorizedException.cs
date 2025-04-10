using System;
using System.Net;

namespace ABCSchool.Application.Exceptions;

public class UnauthorizedException: Exception
{
    public List<string> ErrorMessages { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public UnauthorizedException(
        List<string> errorMessages = default,
        HttpStatusCode statusCode = HttpStatusCode.Unauthorized
    )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
