using System.Collections.Generic;

namespace Audiochan.API.Models
{
    public record ErrorViewModel(int Code, string Message, IDictionary<string, string[]> Errors)
    {
    }
}