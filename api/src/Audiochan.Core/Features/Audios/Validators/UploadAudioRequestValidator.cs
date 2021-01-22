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
        public UploadAudioRequestValidator(IOptions<UploadOptions> uploadSetting)
        {
            RuleFor(req => req.File)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("File is required.")
                .FileValidation(uploadSetting.Value.ContentTypes, uploadSetting.Value.FileSize);
            
            Include(new UpdateAudioRequestValidator());
        }
    }
}