using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Services;
using Audiochan.Core.Features.Users.Exceptions;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using MediatR;

namespace Audiochan.Core.Features.Users.Commands.CreateUser;

public class CreateUserCommand : ICommandRequest<bool>
{
    public CreateUserCommand(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string Username { get; }
    public string Email { get; }
    public string Password { get; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var trimmedUsername = request.Username.Trim();
        if (await _unitOfWork.Users.CheckIfUsernameExists(request.Username, cancellationToken))
            throw new DuplicateUserNameException(request.Username);
        if (await _unitOfWork.Users.CheckIfEmailExists(request.Email, cancellationToken))
            throw new DuplicateEmailException(request.Email);
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(trimmedUsername, request.Email, passwordHash);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}