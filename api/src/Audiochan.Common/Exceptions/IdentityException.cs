namespace Audiochan.Common.Exceptions;

public record IdentityError(string Code, string Message);

public class IdentityException : Exception
{
    public List<IdentityError> Errors { get; }
    public IdentityException(IEnumerable<IdentityError> errors)
        : base("An identity error has occurred.")
    {
        Errors = errors.ToList();
    }
}