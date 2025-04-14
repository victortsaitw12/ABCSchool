using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Identity.Users.Commands;

public class ChangeUserPasswordCommand: IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangeUserPassword {get; set;}
}

public class ChangeUserPasswordCommandHandler: IRequestHandler<ChangeUserPasswordCommand, IResponseWrapper>
{
    private readonly IUserService _userService;
    public ChangeUserPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.ChangePasswordAsync(request.ChangeUserPassword);
        return await ResponseWrapper<string>.SuccessAsync(data:userId, message: "User password changed successfully");
    }
}
