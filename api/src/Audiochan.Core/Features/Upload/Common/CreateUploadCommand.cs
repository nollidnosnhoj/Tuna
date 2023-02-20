using Audiochan.Common.Mediatr;
using Audiochan.Core.Features.Upload.Dtos;

namespace Audiochan.Core.Features.Upload.Common;

public abstract class CreateUploadCommand : ICommandRequest<CreateUploadResponse>
{
    public string FileName { get; }
    public long FileSize { get; }
    public long UserId { get; }

    protected CreateUploadCommand(string fileName, long fileSize, long userId)
    {
        FileName = fileName;
        FileSize = fileSize;
        UserId = userId;
    }
}