using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Audios
{
    public class CreateUploadCommandValidator : AbstractValidator<CreateUploadCommand>
    {
        public CreateUploadCommandValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
        }
    }
}