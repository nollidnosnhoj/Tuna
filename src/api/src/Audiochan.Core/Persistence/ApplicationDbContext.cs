using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Core.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(DbContextOptions options,
            IDateTimeProvider dateTimeProvider, 
            IRandomIdGenerator randomIdGenerator) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
            _randomIdGenerator = randomIdGenerator;
        }

        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<FavoriteAudio> FavoriteAudios { get; set; } = null!;
        public DbSet<FavoritePlaylist> FavoritePlaylists { get; set; } = null!;
        public DbSet<FollowedUser> FollowedUsers { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistAudio> PlaylistAudios { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            // Add default created/updated date
            HandleAuditedEntities();
            
            // Add soft delete property
            HandleSoftDeletion();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("uuid-ossp");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            RenameToSnakeCase(builder);
        }

        public void BeginTransaction()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
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
                
                if (entry.State != EntityState.Deleted) continue;
                
                entry.State = EntityState.Modified;
                entry.Property(nameof(ISoftDeletable.Deleted)).CurrentValue = _dateTimeProvider.Now;
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