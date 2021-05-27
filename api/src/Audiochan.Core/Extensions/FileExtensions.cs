using Microsoft.AspNetCore.StaticFiles;

namespace Audiochan.Core.Extensions
{
    public static class FileExtensions
    {
        private static readonly FileExtensionContentTypeProvider FileExtensionContentTypeProvider = new();

        public static string GetContentType(this string fileName, string? defaultContentType = null)
        {
            if (FileExtensionContentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                return contentType;
            }

            return string.IsNullOrWhiteSpace(defaultContentType)
                ? "application/octet-stream"
                : defaultContentType;
        }
    }
}