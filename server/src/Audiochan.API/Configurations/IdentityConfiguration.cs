using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            var identityOptions = new IdentitySettings();
            configuration.GetSection(nameof(IdentitySettings)).Bind(identityOptions);

            services
                .AddIdentity<User, Role>(options =>
                {
                    options.User.AllowedUserNameCharacters = identityOptions.UsernameAllowedCharacters;
                    options.Password.RequiredLength = identityOptions.PasswordMinimumLength;
                    options.Password.RequireDigit = identityOptions.PasswordRequiresDigit;
                    options.Password.RequireLowercase = identityOptions.PasswordRequiresLowercase;
                    options.Password.RequireUppercase = identityOptions.PasswordRequiresUppercase;
                    options.Password.RequireNonAlphanumeric = identityOptions.PasswordRequiresNonAlphanumeric;
                })
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}