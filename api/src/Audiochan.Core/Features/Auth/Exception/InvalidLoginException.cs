namespace Audiochan.Core.Features.Auth.Exception;

public class InvalidLoginException : System.Exception
{
    public InvalidLoginException()
        : base("Login credentials are invalid.")
    {
        
    }
}