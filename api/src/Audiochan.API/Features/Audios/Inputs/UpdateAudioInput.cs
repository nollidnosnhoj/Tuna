namespace Audiochan.API.Features.Audios.Inputs;

public record UpdateAudioInput(long Id, string? Title, string? Description);