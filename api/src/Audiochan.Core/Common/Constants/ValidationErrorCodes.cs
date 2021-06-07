namespace Audiochan.Core.Common.Constants
{
    public static class ValidationErrorCodes
    {
        public static class Password
        {
            public const string Digits = "requireDigits";
            public const string Lowercase = "requireLowercase";
            public const string Uppercase = "requireUppercase";
            public const string NonAlphanumeric = "requireNonAlphanumeric";
            public const string Length = "requireLength";
        }

        public static class Username
        {
            public const string Characters = "requireCharacters";
            public const string Format = "invalidFormat";
        }
    }
}