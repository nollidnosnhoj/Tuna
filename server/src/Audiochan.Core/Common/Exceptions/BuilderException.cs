using System;

namespace Audiochan.Core.Common.Exceptions
{
    public class BuilderException : Exception
    {
        public BuilderException(string message) : base(message)
        {
            
        }

        public BuilderException() : base("Unable to build entity.")
        {
            
        }
    }
}