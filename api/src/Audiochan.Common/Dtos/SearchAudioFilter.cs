namespace Audiochan.Common.Dtos;

public record SearchAudioFilter(string Term, string Tags, int Page, int Size);