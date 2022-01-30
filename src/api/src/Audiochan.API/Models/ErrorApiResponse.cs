using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Audiochan.API.Models
{
    public record ErrorApiResponse(string? Message, string[]? Errors);
    public record ValidationErrorApiResponse(string? Message, IDictionary<string, string[]>? Errors);
}