using Audiochan.Shared.Models;
using HotChocolate.Types;

namespace Audiochan.API.GraphQL.Types;

public class UserErrorType : InterfaceType<IUserError>
{
    protected override void Configure(IInterfaceTypeDescriptor<IUserError> descriptor)
    {
        descriptor.Name("UserError");
    }
}