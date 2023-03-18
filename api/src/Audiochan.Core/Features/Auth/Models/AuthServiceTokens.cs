using System;

namespace Audiochan.Core.Features.Auth.Models;

public record AuthServiceTokens(string AccessToken, string RefreshToken, DateTime ExpiresAt, IdentityUserDto IdentityUser);