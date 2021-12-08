﻿using Audiochan.Core.Interfaces;
using Audiochan.Core.Services;
using FluentValidation;

namespace Audiochan.Core.Validators
{
    public class ImageDataValidator : AbstractValidator<IImageData>
    {
        private readonly IImageService _imageService;
        
        public ImageDataValidator(IImageService imageService)
        {
            _imageService = imageService;
            RuleFor(x => x.Data)
                .Must(ValidateImage)
                .When(x => !string.IsNullOrEmpty(x.Data))
                .WithMessage("Image must be between 500 and 2000 pixels in both width and height.");
        }

        private bool ValidateImage(string base64)
            => _imageService.ValidateImageSize(base64, 500, 2000);
    }
}