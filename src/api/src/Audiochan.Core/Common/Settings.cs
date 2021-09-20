using System;
using System.Collections.Generic;

namespace Audiochan.Core.Common
{
    public class AuthenticationSettings
    {
        public string DiscordClientId { get; set; } = null!;
        public string DiscordClientSecret { get; set; } = null!;
    }
    
    public record IdentitySettings
    {
        public class UsernameRules
        {
            public int MinimumLength { get; init; } = 3;
            public int MaximumLength { get; init; } = 20;
            public string AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyz0123456789-_";
        }

        public class PasswordRules
        {
            public bool RequiresUppercase { get; init; } = true;
            public bool RequiresLowercase { get; init; } = true;
            public bool RequiresNonAlphanumeric { get; init; } = true;
            public bool RequiresDigit { get; init; } = true;
            public int MinimumLength { get; init; } = 6;
        }

        public UsernameRules UsernameSettings { get; init; } = new();
        public PasswordRules PasswordSettings { get; init; } = new();
    }

    public record MediaStorageSettings
    {
        public AudioStorageSettings Audio { get; init; } = null!;
        public PictureStorageSettings Image { get; init; } = null!;
    }

    public record AudioStorageSettings
    {
        public string Bucket { get; init; } = string.Empty;
        public string TempBucket { get; init; } = string.Empty;
        public List<string> ValidContentTypes { get; init; } = null!;
        public long MaximumFileSize { get; init; }
    }

    public record PictureStorageSettings
    {
        public string Bucket { get; init; } = string.Empty;
        public List<string> ValidContentTypes { get; init; } = null!;
        public long MaximumFileSize { get; init; }
    }
}