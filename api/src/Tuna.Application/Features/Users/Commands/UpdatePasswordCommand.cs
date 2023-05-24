using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Errors;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdatePasswordCommand : ICommandRequest<Result<bool>>
{
    public UpdatePasswordCommand(long userId, string newPassword, string currentPassword)
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public long UserId { get; }
    public string CurrentPassword { get; }
    public string NewPassword { get; }
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<bool>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(UpdatePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new Result<bool>(new UnauthorizedAccessException());

        var result = await _userService.UpdatePasswordAsync(
            user.IdentityId,
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken);

        if (!result.Succeeded) return new Result<bool>(new UserIdentityException(result.Errors));

        // TODO: Remove session

        return true;
    }
}