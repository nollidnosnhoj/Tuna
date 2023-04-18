using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using OneOf.Types;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;
using Tuna.Shared.Errors;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateProfileCommand : ICommandRequest<UpdateProfileResult>
{
    public UpdateProfileCommand(long userId, string? displayName)
    {
        DisplayName = displayName;
        UserId = userId;
    }

    public long UserId { get; }
    public string? DisplayName { get; }
}

[GenerateOneOf]
public partial class UpdateProfileResult : OneOfBase<UserDto, NotFound, Forbidden>
{
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProfileResult> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null) return new NotFound();

        // TODO: Update user stuff

        return new UserDto
        {
            Id = user.Id,
            ImageId = user.ImageId,
            UserName = user.UserName
        };
    }
}