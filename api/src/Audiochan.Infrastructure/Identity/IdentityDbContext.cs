using Audiochan.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Identity;

public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<IdUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");
        base.OnModelCreating(builder);
    }
}