using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Services;
using FluentValidation;

namespace Audiochan.Core.Common.Validators
{
    public class ImageDataValidator : AbstractValidator<IImageData>
    {
        private readonly IImageUploadService _imageUploadService;
        
        public ImageDataValidator(IImageUploadService imageUploadService)
        {
            _imageUploadService = imageUploadService;
            RuleFor(x => x.Data)
                .Must(ValidateImage)
                .When(x => !string.IsNullOrEmpty(x.Data))
                .WithMessage("Image must be between 500 and 2000 pixels in both width and height.");
        }

        private bool ValidateImage(string base64)
            => _imageUploadService.ValidateImageSize(base64, 500, 2000);
    }
}