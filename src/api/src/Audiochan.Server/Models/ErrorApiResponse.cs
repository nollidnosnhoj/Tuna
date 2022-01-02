using System.Collections.Generic;

namespace Audiochan.Server.Models
{
    public record ErrorApiResponse(string? Message, string[]? Errors);
    public record ValidationErrorApiResponse(string? Message, IDictionary<string, string[]>? Errors);
}