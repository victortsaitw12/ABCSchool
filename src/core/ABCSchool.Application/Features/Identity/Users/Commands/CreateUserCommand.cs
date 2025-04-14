using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Identity.Users.Commands;

public class CreateUserCommand: IRequest<IResponseWrapper>
{
    public CreateUserRequest CreateUser {get; set;}
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService;
    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }
    public async Task<IResponseWrapper> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.CreateAsync(request.CreateUser);
        return await ResponseWrapper<string>.SuccessAsync(data:userId, message: "User created successfully");
    }
}
