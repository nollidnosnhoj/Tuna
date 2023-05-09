using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateEmailCommand : ICommandRequest<UpdateEmailCommandResult>
{
    public UpdateEmailCommand(long userId, string newEmail)
    {
        NewEmail = newEmail;
    }

    public long UserId { get; set; }
    public string NewEmail { get; }
}

[GenerateOneOf]
public partial class UpdateEmailCommandResult : OneOfBase<Unit, NotFound, IdentityServiceError>
{
}

public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, UpdateEmailCommandResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmailCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<UpdateEmailCommandResult> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new NotFound();

        var result = await _userService.UpdateEmailAsync(user.IdentityId, command.NewEmail, cancellationToken);

        if (!result.Succeeded) return new IdentityServiceError(result.Errors);

        return Unit.Value;
    }
}