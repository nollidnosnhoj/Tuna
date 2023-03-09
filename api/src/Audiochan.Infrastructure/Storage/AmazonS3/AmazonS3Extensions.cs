using System.Collections.Generic;
using Amazon.S3.Model;

namespace Audiochan.Infrastructure.Storage.AmazonS3;

public static class AmazonS3Extensions
{
    public static void AddMetadata(this MetadataCollection metadataCollection,
        IDictionary<string, string>? inputMeta)
    {
        if (inputMeta == null) return;

        foreach (var (key, value) in inputMeta)
            metadataCollection[key] = value;
    }
}