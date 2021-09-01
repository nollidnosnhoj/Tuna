using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Persistence.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Slug).HasMaxLength(256);
            builder.Property(x => x.Picture).HasMaxLength(256);

            builder.HasIndex(x => x.Created);
            
            builder.HasMany(a => a.Tags)
                .WithMany(t => t.Playlists)
                .UsingEntity(j => j.ToTable("playlist_tags"));
            
            builder.HasMany(a => a.Favorited)
                .WithMany(u => u.FavoritePlaylists)
                .UsingEntity<FavoritePlaylist>(
                    j => j.HasOne(o => o.User)
                        .WithMany()
                        .HasForeignKey(o => o.UserId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(o => o.Playlist)
                        .WithMany()
                        .HasForeignKey(o => o.PlaylistId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(fa => new { fa.PlaylistId, fa.UserId });
                    });
            
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
}