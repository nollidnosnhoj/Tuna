using System.Threading;
using System.Threading.Tasks;
using Tuna.Shared.Mediatr;
using Tuna.Application.Features.Auth;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Persistence;
using Tuna.Application.Services;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateUsernameCommand : ICommandRequest<UpdateUsernameResult>
{
    public long UserId { get; }
    public string NewUserName { get; }

    public UpdateUsernameCommand(long userId, string newUserName)
    {
        UserId = userId;
        NewUserName = newUserName;
    }
}

[GenerateOneOf]
public partial class UpdateUsernameResult : OneOfBase<Unit, NotFound, IdentityServiceError>
{
    
}

public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, UpdateUsernameResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public UpdateUsernameCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<UpdateUsernameResult> Handle(UpdateUsernameCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);
            
        if (user is null)
        {
            return new NotFound();
        }

        var result = await _identityService.UpdateUserNameAsync(
            user.IdentityId,
            command.NewUserName,
            cancellationToken);

        if (!result.Succeeded)
        {
            return new IdentityServiceError(result.Errors);
        }

        user.UserName = command.NewUserName;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}