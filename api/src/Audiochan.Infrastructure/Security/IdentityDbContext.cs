using Audiochan.Infrastructure.Security.Models;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Security;

public class IdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<IdUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    }
}