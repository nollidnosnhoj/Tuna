using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuna.Application.Exceptions;

public class ImageServiceException : Exception
{
    public ImageServiceException(string message) : base(message)
    {
    }
}

public class ImageServiceAggregateException : AggregateException
{
    public ImageServiceAggregateException(IEnumerable<string> errors)
        : this("Multiple errors occurred while processing the image.", errors)
    {
    }

    public ImageServiceAggregateException(string message, IEnumerable<string> errors)
        : base(message, errors.Select(err => new ImageServiceException(err)))
    {
    }
}