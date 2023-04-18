using System;

namespace Tuna.Application.Features.Auth.Models;

public record AuthServiceTokens(string AccessToken, string RefreshToken, DateTime ExpiresAt, IdentityUserDto IdentityUser);