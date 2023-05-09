using Tuna.Domain.Entities.Abstractions;

namespace Tuna.Domain.Entities;

public class User : AuditableEntity<long>
{
    private User()
    {
    }

    public User(string identityId, string userName)
    {
        IdentityId = identityId;
        UserName = userName;
    }

    public string IdentityId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? ImageId { get; set; }
    public ICollection<Audio> Audios { get; set; } = new HashSet<Audio>();
    public ICollection<FavoriteAudio> FavoriteAudios { get; set; } = new HashSet<FavoriteAudio>();
    public ICollection<FollowedUser> Followings { get; set; } = new HashSet<FollowedUser>();
    public ICollection<FollowedUser> Followers { get; set; } = new HashSet<FollowedUser>();

    public void Follow(long observerId, DateTime followedDate)
    {
        var follower = Followers.FirstOrDefault(f => f.ObserverId == observerId);

        if (follower is null)
        {
            follower = new FollowedUser
            {
                TargetId = Id,
                ObserverId = observerId,
                FollowedDate = followedDate
            };

            Followers.Add(follower);
        }
        else if (follower.UnfollowedDate is not null)
        {
            follower.FollowedDate = followedDate;
            follower.UnfollowedDate = null;
        }
    }

    public void UnFollow(long observerId, DateTime unfollowedDate)
    {
        var follower = Followers.FirstOrDefault(f => f.ObserverId == observerId);

        if (follower is not null) follower.UnfollowedDate = unfollowedDate;
    }
}