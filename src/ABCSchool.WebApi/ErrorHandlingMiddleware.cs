using System;
using System.Net;
using System.Text.Json;
using ABCSchool.Application.Exceptions;
using ABCSchool.Application.Wrappers;

namespace ABCSchool.WebApi;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var responseWrapper = ResponseWrapper.Fail();

            switch(ex)
            {
                case ConflictException ce:
                    response.StatusCode = (int)ce.StatusCode;
                    responseWrapper.Messages = ce.ErrorMessages;
                    break;
                case NotFoundException ne:
                    response.StatusCode = (int)ne.StatusCode;
                    responseWrapper.Messages = ne.ErrorMessages;
                    break;
                case ForbiddenException fe:
                    response.StatusCode = (int)fe.StatusCode;
                    responseWrapper.Messages = fe.ErrorMessages;
                    break;
                case IdentityException ie:
                    response.StatusCode = (int)ie.StatusCode;
                    responseWrapper.Messages = ie.ErrorMessages;
                    break;
                case UnauthorizedException ue:
                    response.StatusCode = (int)ue.StatusCode;
                    responseWrapper.Messages = ue.ErrorMessages;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseWrapper.Messages = ["Something went wrong. Contact Administrator"];
                    break;
            }

            var result = JsonSerializer.Serialize(responseWrapper);
            await response.WriteAsync(result);
        }
    }
    
}
