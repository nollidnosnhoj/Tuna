using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audiochan.API.Features.Users;

public static class UserConfiguration
{
    public static IRequestExecutorBuilder AddUserFeature(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<UserNode>()
            .AddTypeExtension<UserQueries>()
            .AddTypeExtension<UserMutations>();
    }
}