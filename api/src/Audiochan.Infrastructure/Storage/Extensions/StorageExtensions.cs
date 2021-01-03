using Microsoft.AspNetCore.StaticFiles;

namespace Audiochan.Infrastructure.Storage.Extensions
{
    public static class StorageExtensions
    {
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();
        
        public static string GetContentType(this string fileName)
        {
            if (!Provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}