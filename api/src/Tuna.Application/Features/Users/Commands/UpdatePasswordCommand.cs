using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Errors;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdatePasswordCommand : ICommandRequest<UpdatePasswordCommandResult>
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

[GenerateOneOf]
public partial class UpdatePasswordCommandResult : OneOfBase<Unit, Unauthorized, IdentityServiceError>
{
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordCommandResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<UpdatePasswordCommandResult> Handle(UpdatePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new Unauthorized();

        var result = await _userService.UpdatePasswordAsync(
            user.IdentityId,
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken);

        if (!result.Succeeded) return new IdentityServiceError(result.Errors);

        // TODO: Remove session

        return Unit.Value;
    }
}