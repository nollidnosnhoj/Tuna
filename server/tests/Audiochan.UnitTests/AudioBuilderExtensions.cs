using System.IO;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Helpers;
using Bogus;

namespace Audiochan.UnitTests
{
    public static class AudioBuilderExtensions
    {
        public static AudioBuilder UseTestDefaults(this AudioBuilder audioBuilder, string userId,
            string fileName = "test.mp3")
        {
            var random = new Randomizer();

            audioBuilder = audioBuilder
                .AddTitle(Path.GetFileNameWithoutExtension(fileName))
                .AddFileName(fileName)
                .AddFileExtension(Path.GetExtension(fileName))
                .AddDuration(random.Number(30, 300))
                .AddFileSize(random.Number(5000, 25000))
                .AddUserId(userId);

            return audioBuilder;
        }
    }
}