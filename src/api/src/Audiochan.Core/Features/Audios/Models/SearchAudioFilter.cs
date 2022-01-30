namespace Audiochan.Core.Features.Audios.Models;

public record SearchAudioFilter(string Term, string Tags, int Page, int Size);