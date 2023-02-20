using Audiochan.Common.Extensions;
using FluentValidation;

namespace Audiochan.Core.Features.Upload.Commands.Images;

public class CreateImageUploadCommandValidator : AbstractValidator<CreateImageUploadCommand>
{
    public CreateImageUploadCommandValidator()
    {
        RuleFor(req => req.FileSize)
            .FileSizeValidation(2097152);
        RuleFor(req => req.FileName)
            .FileNameValidation(new []{"image/jpeg", "image/png", "image/gif"});
    }
}