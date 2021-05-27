using System.Collections.Generic;

namespace Audiochan.API.Features.Audios.GetAudioList
{
    public record GetAudioListViewModel(IReadOnlyList<AudioViewModel> Items, string? Next)
    {
        
    }
}