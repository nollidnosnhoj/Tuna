using System.Collections.Generic;

namespace Audiochan.Core.Common.Models.Requests
{
    public abstract record AudioAbstractRequest
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string Publicity { get; init; }
        public List<string> Tags { get; init; } = new();
    }
}