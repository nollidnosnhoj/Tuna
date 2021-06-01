using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
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
            
            builder.HasMany(x => x.Followers)
                .WithMany(x => x.Followings)
                .UsingEntity<FollowedUser>(
                    j => j.HasOne(x => x.Observer)
                        .WithMany(x => x.FollowingsTable)
                        .HasForeignKey(x => x.ObserverId),
                    j => j.HasOne(x => x.Target)
                        .WithMany(x => x.FollowersTable)
                        .HasForeignKey(x => x.TargetId),
                    j =>
                    {
                        j.HasKey(x => new {x.TargetId, x.ObserverId});
                        j.HasIndex(x => x.FollowedDate);
                        j.HasIndex(x => x.UnfollowedDate);
                        j.HasQueryFilter(x => x.UnfollowedDate == null);
                    });
        }
    }
}