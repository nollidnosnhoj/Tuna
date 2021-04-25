using System.Collections.Generic;

namespace Audiochan.Core.Models.Requests
{
    public abstract class AudioAbstractRequest
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public bool? IsPublic { get; init; }
        public List<string> Tags { get; init; } = new();
    }
}