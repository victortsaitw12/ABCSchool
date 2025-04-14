using System;
using ABCSchool.Application.Features.Identity.Users;
using Microsoft.AspNetCore.Http;

namespace ABCSchool.Infrastructure.Identity;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUserService _currentUserService;

    public CurrentUserMiddleware(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUserService.SetCurrentUser(context.User);
        await next(context);
    }
}
