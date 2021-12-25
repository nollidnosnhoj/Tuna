namespace Audiochan.Application.Commons.Exceptions;

public class UnauthorizedException : BadRequestException
{
    public UnauthorizedException() : base("You are unauthorized.")
    {
    }
}