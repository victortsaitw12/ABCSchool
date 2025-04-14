using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Identity.Users.Queries;

public class GetUserRolesQuery: IRequest<IResponseWrapper>
{
    public string UserId {get; set;}
}

public class GetUserRolesQueryHandler: IRequestHandler<GetUserRolesQuery, IResponseWrapper>
{
    private readonly IUserService _userService;
    public GetUserRolesQueryHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userService.GetUserRolesAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<List<UserRoleResponse>>.SuccessAsync(data: userRoles);
    }

}
