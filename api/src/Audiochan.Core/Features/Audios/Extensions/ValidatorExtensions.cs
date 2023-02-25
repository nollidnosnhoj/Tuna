using Audiochan.Common.Extensions;
using FluentValidation;

namespace Audiochan.Core.Features.Audios.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilder<T, long> AudioFileSizeValidation<T>(this IRuleBuilder<T, long> ruleBuilder)
    {
        return ruleBuilder.FileSizeValidation(MediaConfigurationConstants.AUDIO_MAX_FILE_SIZE);
    }
    
    public static IRuleBuilder<T, string> AudioFileNameValidation<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.FileNameValidation(MediaConfigurationConstants.AUDIO_VALID_TYPES);
    }
}