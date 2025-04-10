using System;
using System.Net;
namespace ABCSchool.Application.Exceptions;

public class IdentityException: Exception
{
 public List<string> ErrorMessages { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public IdentityException(
        List<string> errorMessages = default,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError
    )
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
