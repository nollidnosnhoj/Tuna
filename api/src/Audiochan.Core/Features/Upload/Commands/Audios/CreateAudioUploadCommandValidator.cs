using Audiochan.Common.Extensions;
using FluentValidation;

namespace Audiochan.Core.Features.Upload.Commands.Audios
{
    public class CreateAudioUploadCommandValidator : AbstractValidator<CreateAudioUploadCommand>
    {
        public CreateAudioUploadCommandValidator()
        {
            RuleFor(req => req.FileSize)
                .FileSizeValidation(262144000);
            RuleFor(req => req.FileName)
                .FileNameValidation(new []{"audio/mp3", "audio/mpeg"});
        }
    }
}