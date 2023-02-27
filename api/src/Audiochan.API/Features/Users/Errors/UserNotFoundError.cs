using Audiochan.Common.Models;
using Audiochan.Core.Features.Users.Models;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Users.Errors;

public record UserNotFoundError([ID(nameof(UserViewModel))] long UserId) 
    : UserError("User was not found.", nameof(UserNotFoundError));