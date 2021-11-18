using Audiochan.Core.Common;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
{
    public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            
        }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.UserName).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(256).IsRequired();

            builder.HasDiscriminator(x => x.UserType)
                .HasValue<User>(UserTypes.REGULAR)
                .HasValue<Artist>(UserTypes.ARTIST);

            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
    
    public class FollowedUserConfiguration : IEntityTypeConfiguration<FollowedArtist>
    {
        public void Configure(EntityTypeBuilder<FollowedArtist> builder)
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