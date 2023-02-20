using Audiochan.API.Features.Audios;
using Audiochan.API.Features.Users;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Persistence;
using HashidsNet;
using HotChocolate.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations;

public static class GraphQlConfiguration
{
    public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddAuthorization()
            .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
            .RegisterService<AudioQueryService>()
            .RegisterService<UserQueryService>()
            .RegisterService<IHashids>()
            .RegisterService<IMediator>()
            .AddQueryType()
            .AddMutationType()
            .AddTypeExtension<AudioNode>()
            .AddTypeExtension<UserNode>()
            .AddTypeExtension<AudioQueries>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<AudioMutations>()
            .AddTypeExtension<UserMutations>();

        return services;
    }
}