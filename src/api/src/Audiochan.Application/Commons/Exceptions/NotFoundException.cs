namespace Audiochan.Application.Commons.Exceptions;

public class NotFoundException : BadRequestException
{
    public NotFoundException(string message) : base(message)
    {
        
    }
}

public class NotFoundException<T> : NotFoundException
{
    public NotFoundException() : base($"{typeof(T).Name} was not found.")
    {
        
    }
}