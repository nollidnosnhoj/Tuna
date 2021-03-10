using System.Collections.Generic;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Entities;
using Bogus;

namespace Audiochan.Core.UnitTests.Builders
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

        public AudioBuilder Genre(Genre genre)
        {
            _audio.GenreId = genre.Id;
            _audio.Genre = genre;
            return this;
        }

        public AudioBuilder GenreId(long id)
        {
            _audio.GenreId = id;
            return this;
        }

        public AudioBuilder Public(bool isPublic)
        {
            _audio.IsPublic = isPublic;
            return this;
        }

        public Audio Build()
        {
            return _audio;
        }
    }
}