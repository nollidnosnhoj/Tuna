using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(DbContextOptions options,
            IDateTimeProvider dateTimeProvider, 
            INanoidGenerator nanoid) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<FavoriteAudio> FavoriteAudios { get; set; } = null!;
        public DbSet<FavoritePlaylist> FavoritePlaylists { get; set; } = null!;
        public DbSet<FollowedUser> FollowedUsers { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistAudio> PlaylistAudios { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            // Add default created/updated date
            HandleAuditedEntities();
            
            // Add soft delete property
            HandleSoftDeletion();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        
        public void BeginTransaction()
        {
            if (_currentTransaction != null) return;
            _currentTransaction = Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("uuid-ossp");
            builder.Entity<User>(entity => { entity.ToTable("users"); });
            builder.Entity<Role>(entity => { entity.ToTable("roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("user_roles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("user_claims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("user_logins"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("role_claims"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("user_tokens"); });
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            RenameToSnakeCase(builder);
        }

        private void HandleAuditedEntities()
        {
            foreach (var entry in ChangeTracker.Entries<IAudited>())
            {
                if (entry is null) continue;
                var now = _dateTimeProvider.Now;
                if (entry.State == EntityState.Added && entry.Entity.Created == default)
                {
                    entry.Property(nameof(IAudited.Created)).CurrentValue = now;
                }

                if (entry.State == EntityState.Modified && entry.Entity.LastModified == default)
                {
                    entry.Property(nameof(IAudited.LastModified)).CurrentValue = now;
                }
            }
        }

        private void HandleSoftDeletion()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry is null) continue;
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(ISoftDeletable.Deleted)).CurrentValue = _dateTimeProvider.Now;
                }
            }
        }

        private static void RenameToSnakeCase(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    var storeObjectId = StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema());
                    property.SetColumnName(property.GetColumnName(storeObjectId).ToSnakeCase());
                }

                foreach (var property in entity.GetKeys())
                {
                    property.SetName(property.GetName().ToSnakeCase());
                }

                foreach (var property in entity.GetForeignKeys())
                {
                    property.SetConstraintName(property.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
                }
            }
        }
    }
}