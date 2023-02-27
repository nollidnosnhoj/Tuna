using System.Collections.Generic;
using Audiochan.Common.Models;

namespace Audiochan.Core.Features.Auth.Models;

public record IdentityError(string Code, string Message) : IUserError;

public record NewUserIdentityResult(bool IsSuccess, string IdentityId, IEnumerable<IdentityError> Errors)
    : IdentityResult(IsSuccess, Errors);

public record IdentityResult(bool IsSuccess, IEnumerable<IdentityError> Errors);