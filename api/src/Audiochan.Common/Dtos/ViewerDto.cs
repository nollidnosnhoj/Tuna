namespace Audiochan.Common.Dtos;

public class ViewerDto
{
    
}

public class LoginResult
{
    public LoginResult(string token)
    {
        Token = token;
    }
    
    public string Token { get; }
}