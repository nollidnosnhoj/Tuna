using System;

namespace Audiochan.Core.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum ParseToEnumOrDefault<TEnum>(this string value, TEnum defaultEnum = default)
            where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultEnum;

            if (!Enum.TryParse<TEnum>(value, true, out var result))
                result = defaultEnum;

            return result;
        }
    }
}