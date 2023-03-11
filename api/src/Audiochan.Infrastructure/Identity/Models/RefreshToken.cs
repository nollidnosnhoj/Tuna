using System;

namespace Audiochan.Infrastructure.Identity.Models;

public class RefreshToken
{
    private RefreshToken()
    {
        
    }
    
    public RefreshToken(string token, string userId, string? createdByIp, DateTime createdAt, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        CreatedByIp = createdByIp;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string? CreatedByIp { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsExpired;
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; private set; }
    
    public void Revoke(DateTime revokedAt, string? revokedByIp, string reasonRevoked)
    {
        if (IsRevoked)
            return;
        
        IsRevoked = true;
        RevokedAt = revokedAt;
        RevokedByIp = revokedByIp;
        ReasonRevoked = reasonRevoked;
    }
}