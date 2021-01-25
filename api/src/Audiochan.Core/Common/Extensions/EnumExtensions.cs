using System;

namespace Audiochan.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// This will parse the string into a generic enum. If it cannot parse into an enum, it will return
        /// the default enum type.
        /// </summary>
        /// <param name="value">The string to be parsed into an enum</param>
        /// <param name="defaultEnum">The enum type if failed.</param>
        /// <typeparam name="TEnum">The enumerator type that will be parsed into.</typeparam>
        /// <returns>The enumerator value</returns>
        public static TEnum ParseToEnumOrDefault<TEnum>(this string value, TEnum defaultEnum = default) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultEnum;
            
            if (!Enum.TryParse<TEnum>(value, out var result))
                result = defaultEnum;

            return result;
        }
    }
}