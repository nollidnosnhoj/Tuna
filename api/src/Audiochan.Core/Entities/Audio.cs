using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Extensions;

namespace Audiochan.Core.Entities
{
    public class Audio : IAudited
    {
        public Audio()
        {
            this.Favorited = new HashSet<FavoriteAudio>();
            this.Tags = new HashSet<Tag>();
        }
        
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Duration { get; set; }
        public string BlobName { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public long FileSize { get; set; }
        public string FileExt { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string? PictureBlobName { get; set; }
        public bool IsPublic { get; set; }
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

        public void UpdatePublicity(bool isPublic)
        {
            this.IsPublic = isPublic;
        }

        /// <summary>
        /// Updates the tags for the audio. It will remove tags that are not in the input,
        /// and will add the tags that are in the input, but not in the tags.
        /// </summary>
        /// <param name="tags"></param>
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

        /// <summary>
        /// Determine whether the given user id has the rights to modify. Only the owner of the audio can modify their
        /// audio.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CanModify(string userId)
        {
            return this.UserId == userId;
        }
    }
}