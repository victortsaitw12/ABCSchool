using System;
using System.Net;
namespace ABCSchool.Application.Exceptions;

public class ConflictException: Exception
{
    public List<string> ErrorMessages { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public ConflictException(
        List<string> errorMessages = default,
        HttpStatusCode statusCode = HttpStatusCode.Conflict
    )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
