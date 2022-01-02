using System;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Application.Commons.Exceptions;

public class NotFoundException : BadRequestException
{
    public NotFoundException() : base("Resource was not found.")
    {
        
    }

    public NotFoundException(string message) : base(message)
    {
        
    }
}