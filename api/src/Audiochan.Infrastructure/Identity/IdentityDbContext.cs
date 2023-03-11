using Audiochan.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Identity;

public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<AudiochanIdentityUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");
        builder.Entity<AudiochanIdentityUser>()
            .OwnsMany<RefreshToken>(
                x => x.RefreshTokens, 
                nb => nb.WithOwner().HasForeignKey(x => x.UserId));
        base.OnModelCreating(builder);
    }
}