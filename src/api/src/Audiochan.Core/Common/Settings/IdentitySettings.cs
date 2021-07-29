namespace Audiochan.Core.Common.Settings
{
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
}