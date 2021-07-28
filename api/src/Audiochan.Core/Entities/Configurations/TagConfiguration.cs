using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audiochan.Core.Entities.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property<long>("Id");
            builder.HasKey("Id");
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}