using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Entities.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(x => x.Joined)
                .IsRequired();

            builder.OwnsMany(u => u.RefreshTokens, refreshToken =>
            {
                refreshToken.WithOwner().HasForeignKey(r => r.UserId);
                refreshToken.Property<long>("Id");
                refreshToken.HasKey("Id");
                refreshToken.Property(x => x.Token).IsRequired();
                refreshToken.Property(x => x.Expiry).IsRequired();
                refreshToken.Property(x => x.Created).IsRequired();
                refreshToken.ToTable("refresh_tokens");
            });
        }
    }
    
    public class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedUser>
    {
        public void Configure(EntityTypeBuilder<FollowedUser> builder)
        {
            builder.HasKey(fu => new {FollowerId = fu.ObserverId, FolloweeId = fu.TargetId});

            builder.HasOne(o => o.Observer)
                .WithMany(f => f.Followings)
                .HasForeignKey(o => o.ObserverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Target)
                .WithMany(f => f.Followers)
                .HasForeignKey(o => o.TargetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(f => f.FollowedDate);
            builder.HasQueryFilter(f => f.UnfollowedDate == null);
        }
    }
}