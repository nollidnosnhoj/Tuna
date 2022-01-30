using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Audiochan.Application.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<FavoriteAudio> FavoriteAudios { get; set; } = null!;
        public DbSet<FollowedUser> FollowedUsers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("uuid-ossp");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            RenameToSnakeCase(builder);
        }

        private static void RenameToSnakeCase(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()!.ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    var storeObjectId = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
                    property.SetColumnName(property.GetColumnName(storeObjectId)!.ToSnakeCase());
                }

                foreach (var property in entity.GetKeys())
                {
                    property.SetName(property.GetName()!.ToSnakeCase());
                }

                foreach (var property in entity.GetForeignKeys())
                {
                    property.SetConstraintName(property.GetConstraintName()!.ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
                }
            }
        }
    }
}