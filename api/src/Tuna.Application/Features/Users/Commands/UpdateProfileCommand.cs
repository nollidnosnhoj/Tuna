using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Tuna.Application.Features.Users.Exceptions;
using Tuna.Application.Features.Users.Models;
using Tuna.Application.Persistence;
using Tuna.Domain.Entities;
using Tuna.Domain.Exceptions;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdateProfileCommand : ICommandRequest<Result<UserDto>>
{
    public UpdateProfileCommand(long userId, string? displayName)
    {
        DisplayName = displayName;
        UserId = userId;
    }

    public long UserId { get; }
    public string? DisplayName { get; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(command.UserId, cancellationToken);

        if (user is null)
            return new Result<UserDto>(new UserNotFoundException(command.UserId));

        // TODO: Update user stuff

        return new UserDto
        {
            Id = user.Id,
            ImageId = user.ImageId,
            UserName = user.UserName
        };
    }
}