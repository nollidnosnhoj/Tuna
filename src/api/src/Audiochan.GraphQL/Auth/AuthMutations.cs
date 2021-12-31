using Audiochan.Application.Features.Auth.Commands.Login;
using Audiochan.Application.Features.Auth.Commands.Register;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Services;
using Audiochan.GraphQL.Auth.Errors;
using Audiochan.GraphQL.Common.Errors;
using Audiochan.GraphQL.Users.Errors;
using MediatR;

namespace Audiochan.GraphQL.Auth;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AuthMutations
{
    [UseMutationConvention]
    [Error(typeof(SignInError))]
    public async Task<UserDto> Login(
        string login,
        string password,
        [Service] IMediator mediator,
        [Service] IAuthService authService,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(login, password);
        var user = await mediator.Send(command, cancellationToken);
        await authService.LoginAsync(user, cancellationToken);
        return user;
    }

    public async Task<string> Logout(
        [Service] IAuthService authService,
        CancellationToken cancellationToken = default)
    {
        await authService.LogoutAsync(cancellationToken);
        return "Successfully logged out.";
    }

    [UseMutationConvention(PayloadFieldName = "message")]
    [Error(typeof(ValidationError))]
    [Error(typeof(EmailTaken))]
    [Error(typeof(UsernameTaken))]
    public async Task<string> Register(
        string userName,
        string email,
        string password,
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(userName, email, password);
        await mediator.Send(command, cancellationToken);
        return "Successfully registered.";
    }
}