﻿using Audiochan.GraphQL.Audios;
using Audiochan.GraphQL.Audios.DataLoaders;
using Audiochan.GraphQL.Audios.Errors;
using Audiochan.GraphQL.Common.Errors;
using Audiochan.GraphQL.Users;
using Audiochan.GraphQL.Users.DataLoaders;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.GraphQL;

public static class RegisterGraphQl
{
    public static IServiceCollection AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .InitializeOnStartup()
            .AddMutationConventions(new MutationConventionOptions
            {
                InputTypeNamePattern = "{MutationName}Input",
                PayloadTypeNamePattern = "{MutationName}Payload",
                PayloadErrorTypeNamePattern = "{MutationName}Error"
            })
            .AddAuthorization()
            .AddType<AudioType>()
            .AddType<UserType>()
            .AddQueryType()
            .AddTypeExtension<UserQueries>()
            .AddMutationType()
            .AddTypeExtension<AudioMutations>()
            .AddTypeExtension<UserMutations>()
            .AddDataLoader<AudioByIdDataLoader>()
            .AddDataLoader<UserByIdDataLoader>()
            .AddFiltering()
            .AddSorting()
            .AddGlobalObjectIdentification()
            .AddErrorInterfaceType<IGraphQlError>();

        return services;
    }
}