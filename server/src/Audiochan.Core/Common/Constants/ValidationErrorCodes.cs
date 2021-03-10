namespace Audiochan.Core.Common.Constants
{
    public class ValidationErrorCodes
    {
        public class Password
        {
            public const string RequireDigits = "requireDigits";
            public const string RequireLowercase = "requireLowercase";
            public const string RequireUppercase = "requireUppercase";
            public const string RequireNonAlphanumeric = "requireNonAlphanumeric";
            public const string RequireLength = "requireLength";
        }

        public class Username
        {
            public const string RequireCharacters = "requireCharacters";
        }
    }
}