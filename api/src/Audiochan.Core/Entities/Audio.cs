using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity
    {
        public Audio()
        {
            this.Tags = new HashSet<Tag>();
        }
        
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the audio. Will default to using the original filename without the extension.
        /// </summary>
        public string Title { get; set; } = null!;
        
        /// <summary>
        /// Words to describe the audio. Optional.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// How long the audio is in seconds. Required.
        /// </summary>
        public decimal Duration { get; set; }
        
        /// <summary>
        /// The name of the file in the storage. Required.
        /// </summary>
        public string FileName { get; set; } = null!;
        
        /// <summary>
        /// The name of the file when before being uploaded into the storage. Required.
        /// </summary>
        public string OriginalFileName { get; set; } = null!;
        
        /// <summary>
        /// The size of the audio file in bytes. Required.
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// The file extension of the file uploaded into the storage. Required.
        /// </summary>
        public string FileExt { get; set; } = null!;
        
        /// <summary>
        /// The mime-type of the audio. Required.
        /// </summary>
        public string ContentType { get; set; } = null!;
        
        /// <summary>
        /// The path in the storage where the audio's picture is located. Empty if no picture was uploaded.
        /// </summary>
        public string? Picture { get; set; }

        /// <summary>
        /// Set whether the audio should be listed in public lists or not.
        /// </summary>
        public bool IsPublic { get; set; }
        
        /// <summary>
        /// The foreign key that references the user.
        /// </summary>
        public string UserId { get; set; } = null!;
        
        /// <summary>
        /// The navigation property that references the user.
        /// </summary>
        public User User { get; set; } = null!;
        
        /// <summary>
        /// The navigation property the references the many tags related to this audio.
        /// </summary>
        public ICollection<Tag> Tags { get; set; }

        public void UpdateTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                this.Title = title;
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
                    if (tags.All(t => t.Id != audioTag.Id))
                    {
                        this.Tags.Remove(audioTag);
                    }
                }

                foreach (var tag in tags)
                {
                    if (this.Tags.All(t => t.Id != tag.Id))
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
                this.Picture = picturePath;
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