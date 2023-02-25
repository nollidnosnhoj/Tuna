namespace Audiochan.Core.Features.Auth.Models;

public class LoginResult
{
    public LoginResult(string token)
    {
        Token = token;
    }
    
    public string Token { get; }
}