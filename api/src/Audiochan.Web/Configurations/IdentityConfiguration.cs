using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Web.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, 
            PasswordSetting passwordSetting)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz-_";
                    options.Password.RequiredLength = passwordSetting.RequireLength;
                    options.Password.RequireDigit = passwordSetting.RequireDigit;
                    options.Password.RequireLowercase = passwordSetting.RequireLowercase;
                    options.Password.RequireUppercase = passwordSetting.RequireUppercase;
                    options.Password.RequireNonAlphanumeric = passwordSetting.RequireNonAlphanumeric;
                })
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<AudiochanContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}