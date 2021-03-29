using System.IO;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Common.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioRequestValidator : AbstractValidator<CreateAudioRequest>
    {
        public CreateAudioRequestValidator(IOptions<AudiochanOptions> options)
        {
            var uploadOptions = options.Value.AudioStorageOptions;

            RuleFor(req => req.UploadId)
                .NotEmpty()
                .WithMessage("UploadId is required.");
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .NotEmpty()
                .WithMessage("FileSize is required.")
                .LessThanOrEqualTo(uploadOptions.MaxFileSize)
                .WithMessage("FileSize exceeded maximum file size.");
            RuleFor(req => req.FileName)
                .NotEmpty()
                .WithMessage("Filename is required.")
                .Must(Path.HasExtension)
                .WithMessage("Filename must have a file extension.")
                .Must(fileName => uploadOptions.ContentTypes
                    .Contains(Path.GetExtension(fileName).GetContentType()))
                .WithMessage("Filename is invalid.");

            Include(new AudioAbstractRequestValidator());
        }
    }
}