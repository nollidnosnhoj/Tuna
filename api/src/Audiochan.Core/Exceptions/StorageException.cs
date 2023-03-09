using System;

namespace Audiochan.Core.Exceptions
{
    public class StorageException : Exception
    {
        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StorageException(string msg) : base(msg)
        {
        }
    }
}