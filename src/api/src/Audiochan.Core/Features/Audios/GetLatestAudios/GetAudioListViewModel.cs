using System.Collections.Generic;
using Audiochan.Core.Features.Audios.GetAudio;

namespace Audiochan.Core.Features.Audios.GetLatestAudios
{
    public record GetAudioListViewModel(IReadOnlyList<AudioViewModel> Items, long? Next)
    {
        
    }
}