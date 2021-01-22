namespace Audiochan.Core.Common.Settings
{
    public record PasswordSetting
    {
        public bool RequireUppercase { get; init; } = false;
        public bool RequireLowercase { get; init; } = false;
        public bool RequireNonAlphanumeric { get; init; } = false;
        public bool RequireDigit { get; init; } = false;
        public int RequireLength { get; init; } = 5;
    }
}