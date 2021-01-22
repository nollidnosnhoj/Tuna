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
            _audio = new Audio {Id = Guid.NewGuid().ToString("N")};
        }
        
        public AudioBuilder(IFormFile file) : this()
        {
            _audio.Title = Path.GetFileNameWithoutExtension(file.FileName);
            _audio.FileSize = file.Length;
            _audio.FileExt = Path.GetExtension(file.FileName);
        }

        public string GetId()
        {
            return _audio.Id;
        }

        public string GetBlobName()
        {
            if (string.IsNullOrWhiteSpace(_audio.FileExt))
                return _audio.Id;
            return _audio.Id + _audio.FileExt;
        }

        public AudioBuilder AddTitle(string? title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                _audio.Title = title;
            return this;
        }

        public AudioBuilder AddDescription(string? description)
        {
            _audio.Description = description;
            return this;
        }

        public AudioBuilder AddAudioMetadata(AudioMetadataDto audioMetadata)
        {
            var (_, duration) = audioMetadata;
            _audio.Duration = duration;
            return this;
        }

        public AudioBuilder AddBlobInfo(BlobDto blobDto)
        {
            if (!blobDto.FoundBlob) return this;
            _audio.Url = blobDto.Url;
            if (_audio.FileSize == 0) _audio.FileSize = blobDto.Size;
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
            if (_audio.UserId == 0)
                throw new BuilderException("User is required.");
            if (_audio.GenreId == 0)
                throw new BuilderException("Genre is required.");
            if (string.IsNullOrWhiteSpace(_audio.Url))
                throw new BuilderException("Audio url is required.");
            return _audio;
        }
    }
}