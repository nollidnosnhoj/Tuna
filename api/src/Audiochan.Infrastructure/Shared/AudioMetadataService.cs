using System.IO;
using ATL;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class AudioMetadataService : IAudioMetadataService
    {
        public AudioMetadata GetMetadata(Stream stream, string mimeType)
        {
            var track = new Track(stream, mimeType);

            return new AudioMetadata(track.Title, track.Duration);
        }
    }
}