using SixLabors.ImageSharp;

namespace Audiochan.Common.Helpers;

public static class ImageUtils
{
    public static bool ValidateImageSize(string base64, int min, int max, int? minHeight = null, int? maxHeight = null)
    {
        var info = GetImageInfoFromBase64(base64);
        return info.Width >= min
               && info.Width <= max
               && info.Height >= (minHeight ?? min)
               && info.Height <= (maxHeight ?? max);
    }

    private static IImageInfo GetImageInfoFromBase64(string base64)
    {
        var bytes = EncodingHelpers.ConvertBase64ToBytes(base64);
        var imageInfo = Image.Identify(bytes);
        if (imageInfo is null) throw new Exception("Image Info detector not suitable for image.");
        return imageInfo;
    }
}