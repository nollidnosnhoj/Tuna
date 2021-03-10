using System.Collections.Generic;
using FluentValidation;

namespace Audiochan.Core.Common.Models.Requests
{
    public abstract record AudioCommandRequest
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public bool? IsPublic { get; init; }
        public string Genre { get; init; }
        public List<string> Tags { get; init; } = new();
    }

    public class AudioCommandValidator : AbstractValidator<AudioCommandRequest>
    {
        public AudioCommandValidator()
        {
            // When the title is present, it must be at most 30 characters long.
            RuleFor(req => req.Title)
                .MaximumLength(30)
                .When(req => !string.IsNullOrWhiteSpace(req.Title))
                .WithMessage("Title cannot be no more than 30 characters long.");

            // Description must be at most 500 characters long.
            RuleFor(req => req.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot be more than 500 characters long.");

            // Must be at most 10 tags.
            RuleFor(req => req.Tags)
                .Must(u => u!.Count <= 10)
                .WithMessage("Can only have up to 10 tags per audio upload.");
        }
    }
}