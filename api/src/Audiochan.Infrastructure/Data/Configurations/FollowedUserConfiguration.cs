using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Data.Configurations
{
    public class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedUser>
    {
        public void Configure(EntityTypeBuilder<FollowedUser> builder)
        {
            builder
                .HasKey(fu => new { FollowerId = fu.ObserverId, FolloweeId = fu.TargetId });

            builder
                .HasOne(fu => fu.Observer)
                .WithMany(user => user.Followers)
                .HasForeignKey(fu => fu.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(fu => fu.Target)
                .WithMany(user => user.Followings)
                .HasForeignKey(fu => fu.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
