using System;
using System.Collections.Generic;
using System.Linq;

namespace Audiochan.Domain.Entities;

public sealed class Artist : User
{
    public Artist(string userName, string displayName, string email, string passwordHash)
        : base(userName, email, passwordHash)
    {
        DisplayName = displayName;
    }
    
    public Artist(string userName, string email, string passwordHash) : this(userName, userName, email, passwordHash)
    {
    }
    
    public string DisplayName { get; set; }
    public string? Picture { get; set; }
    public ICollection<Audio> Audios { get; set; } = new HashSet<Audio>();
    public ICollection<FollowedArtist> Followers { get; set; } = new HashSet<FollowedArtist>();
    
    public void Follow(long observerId, DateTime followedDate)
    {
        var follower = this.Followers.FirstOrDefault(f => f.ObserverId == observerId);

        if (follower is null)
        {
            follower = new FollowedArtist
            {
                TargetId = this.Id,
                ObserverId = observerId,
                FollowedDate = followedDate
            };
                
            this.Followers.Add(follower);
        }
        else if (follower.UnfollowedDate is not null)
        {
            follower.FollowedDate = followedDate;
            follower.UnfollowedDate = null;
        }
    }

    public void UnFollow(long observerId, DateTime unfollowedDate)
    {
        var follower = this.Followers.FirstOrDefault(f => f.ObserverId == observerId);

        if (follower is not null)
        {
            follower.UnfollowedDate = unfollowedDate;
        }
    }
}