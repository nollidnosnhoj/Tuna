using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Base;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
    {
        private readonly IDateTimeService _dateTimeService;
        private IDbContextTransaction _currentTransaction;

        public ApplicationDbContext(DbContextOptions options,
            IDateTimeService dateTimeService) : base(options)
        {
            _dateTimeService = dateTimeService;
        }

        public DbSet<Audio> Audios { get; set; }
        public DbSet<FollowedUser> FollowedUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = _dateTimeService.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTimeService.Now;
                        break;
                }
            }

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
            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(u => u.DisplayName)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.Property(x => x.Joined)
                    .IsRequired();

                entity.OwnsMany(u => u.RefreshTokens, refreshToken =>
                {
                    refreshToken.WithOwner().HasForeignKey(r => r.UserId);
                    refreshToken.Property<long>("Id");
                    refreshToken.HasKey("Id");
                    refreshToken.Property(x => x.Token).IsRequired();
                    refreshToken.Property(x => x.Expiry).IsRequired();
                    refreshToken.Property(x => x.Created).IsRequired();
                    refreshToken.ToTable("refresh_tokens");
                });
            });

            builder.Entity<Role>(entity => { entity.ToTable("roles"); });

            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("user_roles"); });

            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("user_claims"); });

            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("user_logins"); });

            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("role_claims"); });

            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("user_tokens"); });

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            RenameToSnakeCase(builder);
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