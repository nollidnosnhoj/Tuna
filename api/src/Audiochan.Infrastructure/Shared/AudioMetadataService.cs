using System.IO;
using ATL;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class AudioMetadataService : IAudioMetadataService
    {
        public AudioMetadataDto GetMetadata(Stream stream, string mimeType)
        {
            var track = new Track(stream, mimeType);

            return new AudioMetadataDto(track.Title, track.Duration);
        }
    }
}