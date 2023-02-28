using Audiochan.Core;
using Audiochan.Core.Features.Users.DataLoaders;
using Audiochan.Core.Features.Users.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Audiochan.API.Features.Users;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        
        descriptor.ImplementsNode()
            .IdField(x => x.Id)
            .ResolveNode(async (context, id) =>
            {
                var dataLoader = context.DataLoader<GetUserDataLoader>();
                return await dataLoader.LoadAsync(id, context.RequestAborted);
            });

        descriptor.Field(x => x.Picture)
            .Name("imageUrl")
            .Resolve(context =>
            {
                var user = context.Parent<UserDto>();
                if (user.Picture is null) return null;
                return MediaLinkConstants.USER_PICTURE + user.Picture;
            });
    }
}

