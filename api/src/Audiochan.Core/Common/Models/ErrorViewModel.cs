using System.Collections.Generic;

namespace Audiochan.Core.Common.Models
{
    public class ErrorViewModel
    {
        public string? Title { get; set; } = string.Empty;
        public string Message { get; set; } = null!;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
