using System.IO;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Upload.GetUploadAudioUrl
{
    public class GetUploadAudioUrlRequestValidator : AbstractValidator<GetUploadAudioUrlRequest>
    {
        public GetUploadAudioUrlRequestValidator(IOptions<AudiochanOptions> options)
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("Filename cannot be empty.")
                .Must(Path.HasExtension)
                .WithMessage("Filename must have a file extension")
                .Must(value => options.Value.AudioStorageOptions.ContentTypes.Contains(value.GetContentType()))
                .WithMessage("The file name's extension is invalid.");
            RuleFor(x => x.FileSize)
                .NotEmpty()
                .WithMessage("Filesize cannot be empty.")
                .Must(value => options.Value.AudioStorageOptions.MaxFileSize >= value)
                .WithMessage("Filesize exceeds maximum limit.");
        }
    }
}