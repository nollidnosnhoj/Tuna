using System.IO;
using ATL;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Shared
{
    public class AudioMetadataService : IAudioMetadataService
    {
        public AudioDataDto GetMetadata(Stream stream, string mimeType)
        {
            var track = new Track(stream, mimeType);
            
            return new AudioDataDto
            {
                Title = track.Title,
                Duration = track.Duration,
                BitRate = track.Bitrate,
                SampleRate = track.SampleRate
            };
        }
    }
}