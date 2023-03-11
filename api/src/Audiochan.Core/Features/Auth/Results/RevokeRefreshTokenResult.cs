using Audiochan.Core.Features.Auth.Errors;
using OneOf;

namespace Audiochan.Core.Features.Auth.Results;

[GenerateOneOf]
public partial class RevokeRefreshTokenResult : OneOfBase<bool, IdentityUserNotFound>
{
    
}