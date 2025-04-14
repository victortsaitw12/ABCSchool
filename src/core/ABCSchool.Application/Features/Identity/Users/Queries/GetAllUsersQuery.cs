using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Identity.Users.Queries;

public class GetAllUsersQuery: IRequest<IResponseWrapper>
{

}

public class GetAllUsersQueryHandler: IRequestHandler<GetAllUsersQuery, IResponseWrapper>
{
    private readonly IUserService _userService;
    public GetAllUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return await ResponseWrapper<List<UserResponse>>.SuccessAsync(data: users);
    }
}