namespace Audiochan.Core.Common.Options
{
    public record IdentityOptions
    {
        public int UsernameMinimumLength { get; init; } = 3;
        public int UsernameMaximumLength { get; init; } = 20;
        public string UsernameAllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyz-_";
        public bool PasswordRequiresUppercase { get; init; } = false;
        public bool PasswordRequiresLowercase { get; init; } = false;
        public bool PasswordRequiresNonAlphanumeric { get; init; } = false;
        public bool PasswordRequiresDigit { get; init; } = false;
        public int PasswordMinimumLength { get; init; } = 5;
    }
}