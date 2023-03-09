using System.Collections.Generic;

namespace Audiochan.Infrastructure.Images.CloudflareImages.Models;

public class CloudflareResponse
{
    public bool Success { get; set; }
    public IReadOnlyList<string> Errors { get; set; } = new List<string>();
    public IReadOnlyList<string> Messages { get; set; } = new List<string>();
}

public class CloudflareResponse<T> : CloudflareResponse
{
    public T? Result { get; set; }
}