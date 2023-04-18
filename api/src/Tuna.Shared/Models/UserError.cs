namespace Tuna.Shared.Models;

public interface IUserError
{
    public string Code { get; }
    public string Message { get; }
}

public class UserError : IUserError
{
    public UserError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }
}