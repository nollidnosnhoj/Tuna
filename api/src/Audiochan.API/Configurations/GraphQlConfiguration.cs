using Audiochan.API.Features.Audios;
using Audiochan.API.Features.Users;
using Audiochan.API.GraphQL.Types;
using Audiochan.Core.Persistence;
using HashidsNet;
using HotChocolate.Data;
using HotChocolate.Execution.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Configurations;

public static class GraphQlConfiguration
{
    public static IServiceCollection AddAudiochanGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer().AddAudiochanSchema();

        return services;
    }
    
    public static IRequestExecutorBuilder AddAudiochanSchema(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddAuthorization()
            .AddMutationConventions()
            .AddGlobalObjectIdentification()
            .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
            .RegisterService<IHashids>()
            .RegisterService<IMediator>()
            .AddQueryType()
            .AddMutationType()
            .AddAudioFeatures()
            .AddUserFeature()
            .AddErrorInterfaceType<UserErrorType>();
    }
}