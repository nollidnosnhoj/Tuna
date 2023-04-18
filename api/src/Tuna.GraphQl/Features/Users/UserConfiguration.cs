using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tuna.Application.Features.Users.DataLoaders;

namespace Tuna.GraphQl.Features.Users;

public static class UserConfiguration
{
    public static IRequestExecutorBuilder AddUserFeature(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<UserNode>()
            .AddTypeExtension<UserExtensions>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<UserMutations>()
            .AddDataLoader<GetUserDataLoader>();
    }
}