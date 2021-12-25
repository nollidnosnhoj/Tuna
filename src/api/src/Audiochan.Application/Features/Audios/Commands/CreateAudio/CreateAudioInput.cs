using System.Collections.Generic;

namespace Audiochan.Application.Features.Audios.Commands.CreateAudio;

public record CreateAudioInput(
    string UploadId,
    string Title,
    string Description,
    List<string> Tags,
    string FileName,
    long FileSize,
    decimal Duration)
{
    public CreateAudioCommand ToCommand(long userId)
    {
        return new CreateAudioCommand(UploadId, Title, Description, Tags, FileName, FileSize, Duration, userId);
    }
}