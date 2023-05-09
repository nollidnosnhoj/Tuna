using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tuna.Application.Services;

public interface IStorageService
{
    string CreateUploadUrl(string bucket, string blobName, TimeSpan expiration, Dictionary<string, string>? metadata = null);
    
    Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default);
}