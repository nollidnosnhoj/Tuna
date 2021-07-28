using System;
using System.Text;
using Audiochan.Core;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.API.Extensions.ConfigurationExtensions
{
    public static class AuthConfigExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new IdentitySettings();
            configuration.GetSection(nameof(IdentitySettings)).Bind(settings);

            services
                .AddIdentity<User, Role>(options =>
                {
                    options.User.AllowedUserNameCharacters = settings.UsernameSettings.AllowedCharacters;
                    options.Password.RequiredLength = settings.PasswordSettings.MinimumLength;
                    options.Password.RequireDigit = settings.PasswordSettings.RequiresDigit;
                    options.Password.RequireLowercase = settings.PasswordSettings.RequiresLowercase;
                    options.Password.RequireUppercase = settings.PasswordSettings.RequiresUppercase;
                    options.Password.RequireNonAlphanumeric = settings.PasswordSettings.RequiresNonAlphanumeric;
                })
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSetting = new JwtSettings();
            configuration.GetSection(nameof(JwtSettings)).Bind(jwtSetting);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParams);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParams;
                    options.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.RefreshTokenSecret));
                });

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }
    }
}