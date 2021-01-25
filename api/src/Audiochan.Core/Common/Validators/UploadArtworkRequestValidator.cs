using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Common.Validators
{
    public class UploadArtworkRequestValidator : AbstractValidator<UploadArtworkRequest>
    {
        public UploadArtworkRequestValidator(IOptions<AudiochanOptions> options)
        {
            var imageUploadOptions = options.Value.ImageUploadOptions;

            RuleFor(x => x.Image)
                .FileValidation(imageUploadOptions.FileExtensions, imageUploadOptions.FileSize);
        }
    }
}