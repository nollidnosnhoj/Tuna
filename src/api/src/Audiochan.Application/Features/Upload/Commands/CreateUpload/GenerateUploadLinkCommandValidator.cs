using Audiochan.Application.Commons.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Upload.Commands.CreateUpload
{
    public class GenerateUploadLinkCommandValidator : AbstractValidator<GenerateUploadLinkCommand>
    {
        public GenerateUploadLinkCommandValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
        }
    }
}