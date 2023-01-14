namespace Audiochan.Common;

public static class ValidationErrorCodes
{
    public static class Password
    {
        public const string DIGITS = "requireDigits";
        public const string LOWERCASE = "requireLowercase";
        public const string UPPERCASE = "requireUppercase";
        public const string NON_ALPHANUMERIC = "requireNonAlphanumeric";
        public const string LENGTH = "requireLength";
    }

    public static class Username
    {
        public const string CHARACTERS = "requireCharacters";
        public const string FORMAT = "invalidFormat";
    }
}