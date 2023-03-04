using Audiochan.Shared.Mediatr;
using Audiochan.Core.Features.Upload.Models;

namespace Audiochan.Core.Features.Upload.Commands;

public abstract class CreateUploadCommand : ICommandRequest<CreateUploadResult>
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