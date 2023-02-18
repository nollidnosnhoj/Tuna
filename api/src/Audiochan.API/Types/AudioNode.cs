using Audiochan.Core;
using Audiochan.Core.Features.Audios.Dtos;
using HashidsNet;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Types;

[ExtendObjectType<AudioDto>]
public class AudioNode
{
    public string GetSlug([Parent] AudioDto audio, IHashids hashids)
    {
        return hashids.EncodeLong(audio.Id);
    }

    [BindMember(nameof(AudioDto.ObjectKey))]
    public string GetStreamUrl([Parent] AudioDto audio)
    {
        return MediaLinkConstants.AUDIO_STREAM + audio.ObjectKey;
    }

    [BindMember(nameof(AudioDto.Picture))]
    public string? GetImageUrl([Parent] AudioDto audio)
    {
        if (audio.Picture is null) return null;
        return MediaLinkConstants.AUDIO_PICTURE + audio.Picture;
    }
}