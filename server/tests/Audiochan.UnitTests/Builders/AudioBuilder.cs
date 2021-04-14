using System.Collections.Generic;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Audiochan.Core.Helpers;
using Bogus;

namespace Audiochan.UnitTests.Builders
{
    public class AudioBuilder
    {
        private Audio _audio;
        private readonly Randomizer _randomizer = new();

        public AudioBuilder(string userId, string fileName = "test.mp3")
        {
            _audio = new Audio(
                uploadId: UploadHelpers.GenerateUploadId(),
                fileName: fileName,
                fileSize: _randomizer.Number(5000, 25000),
                duration: _randomizer.Number(30, 300),
                userId: userId);
        }

        public AudioBuilder Title(string title)
        {
            _audio.Title = title;
            return this;
        }

        public AudioBuilder Description(string description)
        {
            _audio.Description = description;
            return this;
        }

        public AudioBuilder Tags(List<Tag> tags)
        {
            _audio.Tags = tags;
            return this;
        }

        public AudioBuilder Publicity(Visibility status, string privateKey = "")
        {
            _audio.Visibility = status;
            if (_audio.Visibility == Visibility.Private && !string.IsNullOrEmpty(privateKey))
                _audio.PrivateKey = privateKey;
            return this;
        }

        public Audio Build()
        {
            return _audio;
        }
    }
}