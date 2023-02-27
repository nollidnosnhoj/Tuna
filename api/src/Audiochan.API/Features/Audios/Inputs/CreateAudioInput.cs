namespace Audiochan.API.Features.Audios.Inputs;

public record CreateAudioInput(
    string UploadId,
    string FileName,
    long FileSize,
    decimal Duration,
    string Title,
    string Description);