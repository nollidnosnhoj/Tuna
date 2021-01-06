using System.Collections.Generic;
using System.Text;

namespace Audiochan.Core.Common.Models
{
    public record ErrorViewModel(string? Title, string Message, IDictionary<string, string[]>? Errors)
    {
    }
}
