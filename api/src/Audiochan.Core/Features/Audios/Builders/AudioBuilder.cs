using System;
using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Features.Audios.Builders
{
    public class AudioBuilder
    {
        private readonly Audio _audio;

        public AudioBuilder()
        {
            _audio = new Audio {Id = Guid.NewGuid().ToString()};
        }

        public string GetId()
        {
            return _audio.Id;
        }

        public AudioBuilder AddTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                _audio.Title = title;
            return this;
        }

        public AudioBuilder AddDescription(string description)
        {
            _audio.Description = description;
            return this;
        }

        public AudioBuilder AddUploadResult(AudioUploadResult uploadResult)
        {
            _audio.Duration = uploadResult.Duration;
            _audio.FileSize = uploadResult.FileSize;
            _audio.FileExt = uploadResult.FileExt;
            _audio.StreamUrl = uploadResult.StreamUrl;
            return this;
        }

        public AudioBuilder AddGenre(Genre genre)
        {
            _audio.GenreId = genre.Id;
            _audio.Genre = genre;
            return this;
        }

        public AudioBuilder AddTags(ICollection<Tag> tags)
        {
            _audio.Tags = tags;
            return this;
        }

        public AudioBuilder AddUser(User user)
        {
            _audio.UserId = user.Id;
            _audio.User = user;
            return this;
        }

        public AudioBuilder AddImage(BlobDto blob)
        {
            _audio.PictureUrl = blob?.Url ?? string.Empty;
            return this;
        }

        public AudioBuilder SetToPublic(bool? isPublic)
        {
            _audio.IsPublic = isPublic ?? true;
            return this;
        }

        public AudioBuilder SetToLoop(bool? isLoop)
        {
            _audio.IsLoop = isLoop ?? true;
            return this;
        }

        public Audio Build()
        {
            if (string.IsNullOrWhiteSpace(_audio.Title))
                throw new BuilderException("Title is required.");
            if (string.IsNullOrWhiteSpace(_audio.UserId))
                throw new BuilderException("User is required.");
            if (_audio.GenreId == 0)
                throw new BuilderException("Genre is required.");
            if (string.IsNullOrWhiteSpace(_audio.StreamUrl))
                throw new BuilderException("Audio url is required.");
            return _audio;
        }
    }
}