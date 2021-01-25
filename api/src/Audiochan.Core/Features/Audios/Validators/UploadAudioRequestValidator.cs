using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Audios.Models;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.Validators
{
    public class UploadAudioRequestValidator : AbstractValidator<UploadAudioRequest>
    {
        public UploadAudioRequestValidator(IOptions<AudiochanOptions> options)
        {
            var uploadOptions = options.Value.AudioUploadOptions;
            RuleFor(req => req.File)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("File is required.")
                .FileValidation(uploadOptions.FileExtensions, uploadOptions.FileSize);
            
            Include(new UpdateAudioRequestValidator());
        }
    }
}