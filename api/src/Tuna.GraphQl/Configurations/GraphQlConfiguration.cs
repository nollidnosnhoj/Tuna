using Tuna.GraphQl.Features.Audios;
using Tuna.GraphQl.Features.Users;
using Tuna.Application.Persistence;
using HashidsNet;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Execution.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tuna.GraphQl.GraphQL.Types;

namespace Tuna.GraphQl.Configurations;

public static class GraphQlConfiguration
{
    public static IServiceCollection AddTunaGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer().AddTunaSchema();

        return services;
    }
    
    public static IRequestExecutorBuilder AddTunaSchema(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddAuthorization()
            .AddMutationConventions()
            .AddGlobalObjectIdentification()
            .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled)
            .RegisterService<IHashids>()
            .RegisterService<IMediator>(ServiceKind.Resolver)
            .AddQueryType()
            .AddMutationType()
            .AddAudioFeatures()
            .AddUserFeature()
            .AddErrorInterfaceType<UserErrorType>();
    }
}