namespace Audiochan.Core.Dtos.Filters;

public record SearchAudioFilter(string Term, string Tags, int Page, int Size);