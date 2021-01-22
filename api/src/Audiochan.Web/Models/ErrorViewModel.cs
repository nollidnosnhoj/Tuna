using System.Collections.Generic;

namespace Audiochan.Web.Models
{
    public record ErrorViewModel(string? Title, string Message, IDictionary<string, string[]>? Errors)
    {
    }
}
