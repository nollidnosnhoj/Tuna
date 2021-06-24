using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class Audio : IAudited
    {
        public Audio()
        {
            this.Favorited = new HashSet<FavoriteAudio>();
            this.Tags = new HashSet<Tag>();
        }
        
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Duration { get; set; }
        public string BlobName { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public long FileSize { get; set; }
        public string? PictureBlobName { get; set; }
        public Visibility Visibility { get; set; }
        public string? PrivateKey { get; set; }
        public string UserId { get; set; } = null!;
        
        public User User { get; set; } = null!;
        public ICollection<FavoriteAudio> Favorited { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public void UpdateTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                this.Title = title;
            }
        }

        public void UpdateDescription(string? description)
        {
            if (description is not null)
                this.Description = description;
        }

        public void UpdateVisibility(Visibility visibility)
        {
            var old = this.Visibility;
            this.Visibility = visibility;

            if (old == Visibility.Private && this.Visibility != Visibility.Private)
            {
                this.PrivateKey = null;
            }
            else if (old != Visibility.Private && this.Visibility == Visibility.Private)
            {
                this.PrivateKey = GeneratePrivateKey();
            }
        }

        public void ResetPrivateKey()
        {
            if (this.Visibility == Visibility.Private)
            {
                this.PrivateKey = GeneratePrivateKey();
            }
        }
        
        public void UpdateTags(List<Tag> tags)
        {
            if (this.Tags.Count > 0)
            {
                foreach (var audioTag in this.Tags)
                {
                    if (tags.All(t => t.Name != audioTag.Name))
                    {
                        this.Tags.Remove(audioTag);
                    }
                }

                foreach (var tag in tags)
                {
                    if (this.Tags.All(t => t.Name != tag.Name))
                        this.Tags.Add(tag);
                }
            }
            else
            {
                foreach (var tag in tags)
                {
                    this.Tags.Add(tag);
                }
            }
        }

        public void UpdatePicture(string picturePath)
        {
            if (!string.IsNullOrWhiteSpace(picturePath))
                this.PictureBlobName = picturePath;
        }

        public bool CanModify(string userId)
        {
            return this.UserId == userId;
        }

        private string GeneratePrivateKey()
        {
            return Nanoid.Nanoid.Generate(size: 8);
        }
    }
}