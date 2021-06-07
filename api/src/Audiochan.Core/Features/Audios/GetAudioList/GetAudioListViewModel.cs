using System.Collections.Generic;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record GetAudioListViewModel(IReadOnlyList<AudioViewModel> Items, string? Next)
    {
        
    }
}