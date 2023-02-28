using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Errors;
using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Audiochan.Core.Features.Users.Commands;

public class UpdateProfileCommand : ICommandRequest<UpdateProfileResult>
{
    public long UserId { get; }
    public string? DisplayName { get; }

    public UpdateProfileCommand(long userId, string? displayName)
    {
        DisplayName = displayName;
        UserId = userId;
    }
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
            Picture = user.ImageId,
            UserName = user.UserName
        };
    }
}