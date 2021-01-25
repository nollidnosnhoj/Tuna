using System.Collections.Generic;

namespace Audiochan.Web.Models
{
    public record ErrorViewModel(int Code, string Message, IDictionary<string, string[]> Errors) { }
}
