using Audiochan.Application.Commons.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Audios.Commands.CreateAudio
{
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        public CreateAudioCommandValidator(IOptions<MediaStorageSettings> options)
        {
            var storageSettings = options.Value;
            RuleFor(req => req.UploadId)
                .NotEmpty();
            RuleFor(req => req.Duration)
                .NotEmpty();
            RuleFor(req => req.FileSize)
                .FileSizeValidation(storageSettings.Audio.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(storageSettings.Audio.ValidContentTypes);
            RuleFor(req => req.Title)
                .NotEmpty()
                .MaximumLength(30);
            RuleFor(req => req.Description)
                .NotNull()
                .MaximumLength(500);
            RuleFor(req => req.Tags)
                .NotNull()
                .Must(u => u!.Length <= 10)
                .ForEach(tagsRule =>
                {
                    tagsRule
                        .NotEmpty()
                        .Length(3, 15);
                });
        }
    }
}