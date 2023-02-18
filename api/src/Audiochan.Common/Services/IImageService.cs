namespace Audiochan.Common.Services
{
    public interface IImageService
    {
        Task UploadImage(string data, string container, string blobName, CancellationToken cancellationToken = default);
        Task RemoveImage(string container, string blobName, CancellationToken cancellationToken = default);
    }
}