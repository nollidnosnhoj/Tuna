using Audiochan.Common.Helpers;
using Audiochan.Common.Interfaces;
using Audiochan.Common.Services;
using FluentValidation;

namespace Audiochan.Core.Validators
{
    public class ImageDataValidator : AbstractValidator<IImageData>
    {
        public ImageDataValidator(IImageService imageService)
        {
            RuleFor(x => x.Data)
                .Must(ValidateImage)
                .When(x => !string.IsNullOrEmpty(x.Data))
                .WithMessage("Image must be between 500 and 2000 pixels in both width and height.");
        }

        private bool ValidateImage(string? base64)
        {
            if (string.IsNullOrEmpty(base64)) return true;
            return ImageUtils.ValidateImageSize(base64, 500, 2000);
        }
    }
}