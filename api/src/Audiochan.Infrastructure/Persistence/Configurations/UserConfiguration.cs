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

            builder.HasMany(u => u.Followers)
                .WithMany(u => u.Followings)
                .UsingEntity(u => u.ToTable("followed_users"));

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
}