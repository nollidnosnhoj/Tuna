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

namespace Audiochan.Infrastructure.Data
{
    public class DatabaseContext : IdentityDbContext<User, Role, long>, IDatabaseContext
    {
        private readonly IDateTimeService _dateTimeService;
        
        public DatabaseContext(DbContextOptions options, 
            IDateTimeService dateTimeService) : base(options)
        {
            _dateTimeService = dateTimeService;
        }

        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<FavoriteAudio> FavoriteAudios { get; set; } = null!;
        public DbSet<FollowedUser> FollowedUsers { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(u => u.DisplayName).HasMaxLength(256);
                
                entity.OwnsMany(u => u.RefreshTokens, refreshToken =>
                {
                    refreshToken.WithOwner().HasForeignKey(r => r.UserId);
                    refreshToken.Property<long>("Id");
                    refreshToken.HasKey("Id");
                    refreshToken.ToTable("refresh_tokens");
                });
            });
            
            builder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
            });
            
            builder.Entity<IdentityUserRole<long>>(entity =>
            {
                entity.ToTable("user_roles");
            });
            
            builder.Entity<IdentityUserClaim<long>>(entity =>
            {
                entity.ToTable("user_claims");
            });
            
            builder.Entity<IdentityUserLogin<long>>(entity =>
            {
                entity.ToTable("user_logins");
            });
            
            builder.Entity<IdentityRoleClaim<long>>(entity =>
            {
                entity.ToTable("role_claims");
            });
            
            builder.Entity<IdentityUserToken<long>>(entity =>
            {
                entity.ToTable("user_tokens");
            });

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
