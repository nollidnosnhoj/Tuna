using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Exceptions;
using Tuna.Application.Features.Users.Exceptions;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateEmailCommand : ICommandRequest<Result<bool>>
{
    public UpdateEmailCommand(long userId, string newEmail)
    {
        UserId = userId;
        NewEmail = newEmail;
    }

    public long UserId { get; set; }
    public string NewEmail { get; }
}

public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, Result<bool>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmailCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<Result<bool>> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null)
            return new Result<bool>(new UserNotFoundException(command.UserId));

        var result = await _userService.UpdateEmailAsync(user.IdentityId, command.NewEmail, cancellationToken);

        if (!result.Succeeded)
            return new Result<bool>(new UserIdentityException(result.Errors));

        return true;
    }
}