namespace Audiochan.Core.Features.Auth.Dtos;

public class LoginRequest
{
    public LoginRequest(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; }
    public string Password { get; }
}