using Tuna.Shared.Models;
using HotChocolate.Types;

namespace Tuna.GraphQl.GraphQL.Types;

public class UserErrorType : InterfaceType<IUserError>
{
    protected override void Configure(IInterfaceTypeDescriptor<IUserError> descriptor)
    {
        descriptor.Name("UserError");
    }
}