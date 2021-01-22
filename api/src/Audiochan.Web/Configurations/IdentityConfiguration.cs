using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;

namespace Audiochan.Web.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var identityOptions = new IdentityUserOptions();
            configuration.GetSection(nameof(IdentityUserOptions)).Bind(identityOptions);
            
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
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}