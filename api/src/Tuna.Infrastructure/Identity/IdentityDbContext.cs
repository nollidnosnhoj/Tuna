using Microsoft.EntityFrameworkCore;
using Tuna.Infrastructure.Identity.Models;

namespace Tuna.Infrastructure.Identity;

public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<AuthUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");
        builder.Entity<AuthUser>()
            .OwnsMany<RefreshToken>(
                x => x.RefreshTokens, 
                nb => nb.WithOwner().HasForeignKey(x => x.UserId));
        base.OnModelCreating(builder);
    }
}