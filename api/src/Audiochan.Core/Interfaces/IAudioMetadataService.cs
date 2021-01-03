using System.IO;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IAudioMetadataService
    {
        AudioDataDto GetMetadata(Stream stream, string mimeType);
    }
}