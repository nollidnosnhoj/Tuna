using System;
using System.Text;
using Audiochan.Core.Entities;
using Audiochan.Core.Settings;
using Audiochan.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.API.Extensions
{
    public static class AuthConfigExtensions
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
        
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSetting = new JwtSettings();
            configuration.GetSection(nameof(JwtSettings)).Bind(jwtSetting);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
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