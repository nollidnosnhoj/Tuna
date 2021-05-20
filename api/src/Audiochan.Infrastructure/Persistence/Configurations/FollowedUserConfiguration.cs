using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
{
    public class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedUser>
    {
        public void Configure(EntityTypeBuilder<FollowedUser> builder)
        {
            builder
                .HasKey(fu => new {FollowerId = fu.ObserverId, FolloweeId = fu.TargetId});

            builder.HasOne(o => o.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(o => o.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(o => o.TargetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}