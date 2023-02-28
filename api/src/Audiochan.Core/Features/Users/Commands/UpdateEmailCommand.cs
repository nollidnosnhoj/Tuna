using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Users.Errors;
using Audiochan.Core.Persistence;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Audiochan.Core.Features.Users.Commands;

public class UpdateEmailCommand : ICommandRequest<UpdateEmailCommandResult>
{
    public long UserId { get; set; }
    public string NewEmail { get; }

    public UpdateEmailCommand(long userId, string newEmail)
    {
        NewEmail = newEmail;
    }
}

[GenerateOneOf]
public partial class UpdateEmailCommandResult : OneOfBase<Unit, NotFound, IdentityServiceError>
{
    
}

public class UpdateEmailCommandHandler : IRequestHandler<UpdateEmailCommand, UpdateEmailCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public UpdateEmailCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<UpdateEmailCommandResult> Handle(UpdateEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new NotFound();

        var result = await _identityService.UpdateEmailAsync(user.IdentityId, command.NewEmail, cancellationToken);

        if (!result.Succeeded)
        {
            return new IdentityServiceError(result.Errors);
        }

        return Unit.Value;
    }
}