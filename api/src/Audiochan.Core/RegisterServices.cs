using System.Reflection;
using Audiochan.Core.Features.Favorites;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Followers;
using Audiochan.Core.Features.Genres;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.Core
{
    public static class RegisterServices
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFollowerService, FollowerService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}