namespace Audiochan.Core.Interfaces
{
    public interface IUploadService
    {
        (string UploadId, string Url) GetUploadUrl(string fileName);
    }
}