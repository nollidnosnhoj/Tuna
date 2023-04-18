using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuna.Application.Features.Auth.Results;

public record AuthServiceError(string Code, string Description);

public class AuthServiceResult
{
    protected readonly bool _succeeded;
    protected readonly List<AuthServiceError> _errors = new();
    
    public bool Succeeded => _succeeded;
    public List<AuthServiceError> Errors => _errors;
    
    protected AuthServiceResult()
    {
        _succeeded = true;
    }
    
    protected AuthServiceResult(params AuthServiceError[] errors)
    {
        _succeeded = false;
        _errors.AddRange(errors);
    }

    protected AuthServiceResult(IEnumerable<AuthServiceError> errors)
    {
        _succeeded = false;
        _errors.AddRange(errors);
    }

    public static AuthServiceResult Succeed()
    {
        return new AuthServiceResult();
    }

    public static AuthServiceResult Failed(params AuthServiceError[] errors)
    {
        return new AuthServiceResult(errors);
    }
    
    public static implicit operator AuthServiceResult(AuthServiceError error) => new(error);
    public static implicit operator AuthServiceResult(List<AuthServiceError> errors) => new(errors);
}

public class AuthServiceResult<T> : AuthServiceResult
{
    private readonly T _data = default!;
    public T Result => _succeeded ? _data : throw new AuthResultException(_errors);
    
    private AuthServiceResult(T data)
    {
        _data = data;
    }
    
    private AuthServiceResult(params AuthServiceError[] errors) : base(errors)
    {
    }

    private AuthServiceResult(IEnumerable<AuthServiceError> errors) : base(errors)
    {
    }

    public static AuthServiceResult<T> Succeed(T data)
    {
        return new AuthServiceResult<T>(data);
    }

    public new static AuthServiceResult<T> Failed(params AuthServiceError[] errors)
    {
        return new AuthServiceResult<T>(errors);
    }

    public static implicit operator AuthServiceResult<T>(T _) => new(_);
    public static implicit operator AuthServiceResult<T>(AuthServiceError error) => new(error);
    public static implicit operator AuthServiceResult<T>(List<AuthServiceError> errors) => new(errors);
}

public class AuthResultException : Exception
{
    public AuthResultException(string message) : base(message)
    {
        
    }

    public AuthResultException(List<AuthServiceError> errors)
        : base($"Authentication failed: {string.Join(", ", errors.Select(e => e.Description))}")
    {
        
    }
}