using System;

namespace Audiochan.Core.Features.Auth.Models;

public record AuthTokenResult(string AccessToken, string RefreshToken, DateTime ExpiresAt);