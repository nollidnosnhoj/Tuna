using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Infrastructure.Persistence.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Picture).HasMaxLength(256);

            builder.HasIndex(x => x.Title);
            builder.HasIndex(x => x.Tags).HasMethod("GIN");
            builder.HasIndex(x => x.Created);

            builder.HasMany(p => p.Audios)
                .WithMany(a => a.Playlists)
                .UsingEntity<PlaylistAudio>(
                    j => j.HasOne(pa => pa.Audio)
                        .WithMany()
                        .HasForeignKey(o => o.AudioId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(pa => pa.Playlist)
                        .WithMany(p => p.PlaylistAudios)
                        .HasForeignKey(o => o.PlaylistId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(x => x.Id);
                        j.Property(x => x.Id).ValueGeneratedOnAdd(); 
                        j.HasIndex(x => new { x.PlaylistId, x.AudioId });
                    });

            builder.HasOne(x => x.User)
                .WithMany(x => x.Playlists)
                .IsRequired()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
    public class FavoritePlaylistConfiguration : IEntityTypeConfiguration<FavoritePlaylist>
    {
        public void Configure(EntityTypeBuilder<FavoritePlaylist> builder)
        {
            builder.HasKey(x => new {x.PlaylistId, x.UserId});
            builder.HasOne(x => x.User)
                .WithMany(u => u.FavoritePlaylists)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Playlist)
                .WithMany()
                .HasForeignKey(fa => fa.PlaylistId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}