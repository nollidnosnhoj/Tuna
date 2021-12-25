namespace Audiochan.Application.Commons.Exceptions;

public class ForbiddenException : BadRequestException
{
    public ForbiddenException() : base("Forbidden.")
    {
    }
}