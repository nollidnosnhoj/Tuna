using Audiochan.Core;
using Audiochan.Core.Features.Audios.Models;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Features.Audios;

[ExtendObjectType<AudioViewModel>]
public class AudioNode
{
    public string GetSlug([Parent] AudioViewModel audio, IHashids hashids)
    {
        return hashids.EncodeLong(audio.Id);
    }

    [BindMember(nameof(AudioViewModel.ObjectKey))]
    public string GetStreamUrl([Parent] AudioViewModel audio)
    {
        return MediaLinkConstants.AUDIO_STREAM + audio.ObjectKey;
    }

    [BindMember(nameof(AudioViewModel.Picture))]
    public string? GetImageUrl([Parent] AudioViewModel audio)
    {
        if (audio.Picture is null) return null;
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}