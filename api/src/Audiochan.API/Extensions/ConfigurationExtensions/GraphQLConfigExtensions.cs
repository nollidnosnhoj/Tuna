using Audiochan.API.Queries;
using Audiochan.API.Types;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Persistence;
using HashidsNet;
using HotChocolate.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Extensions.ConfigurationExtensions;

public static class GraphQLConfigExtensions
{
    public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
    {
        services.AddGraphQLServer()
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
            .AddTypeExtension<UserQueries>();

        return services;
    }
}