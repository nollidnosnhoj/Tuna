namespace Audiochan.Core.Constants
{
    public static class ValidationErrorCodes
    {
        public static class Password
        {
            public const string RequireDigits = "requireDigits";
            public const string RequireLowercase = "requireLowercase";
            public const string RequireUppercase = "requireUppercase";
            public const string RequireNonAlphanumeric = "requireNonAlphanumeric";
            public const string RequireLength = "requireLength";
        }

        public static class Username
        {
            public const string RequireCharacters = "requireCharacters";
            public const string InvalidFormat = "invalidFormat";
        }
    }
}