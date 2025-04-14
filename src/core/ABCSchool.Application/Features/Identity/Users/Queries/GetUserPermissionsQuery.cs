using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Identity.Users.Queries;

public class GetUserPermissionsQuery: IRequest<IResponseWrapper>
{
    public string UserId {get; set;}
}

public class GetUserPermissionsQueryHandler: IRequestHandler<GetUserPermissionsQuery, IResponseWrapper>
{
    private readonly IUserService _userService;
    public GetUserPermissionsQueryHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _userService.GetUserPermissionsAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<List<string>>.SuccessAsync(data: permissions);
    }
}

