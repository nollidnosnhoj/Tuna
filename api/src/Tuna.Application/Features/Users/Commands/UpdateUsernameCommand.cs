using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Audios.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateUsernameCommand : ICommandRequest<Result<bool>>
{
    public UpdateUsernameCommand(long userId, string newUserName)
    {
        UserId = userId;
        NewUserName = newUserName;
    }

    public long UserId { get; }
    public string NewUserName { get; }
}

public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, Result<bool>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUsernameCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null)
            return new Result<bool>(new AudioNotFoundException(command.UserId));

        var result = await _userService.UpdateUserNameAsync(
            user.IdentityId,
            command.NewUserName,
            cancellationToken);

        if (!result.Succeeded)
            return new Result<bool>(new UserIdentityException(result.Errors));

        user.UserName = command.NewUserName;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}