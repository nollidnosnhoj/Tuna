using System.Collections.Generic;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace Audiochan.Infrastructure.Storage.Extensions
{
    public static class AmazonS3Extensions
    {
        public static void AddMetadataCollection(this PutObjectRequest request, Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }

        public static void AddMetadataCollection(this TransferUtilityUploadRequest request,
            Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }

        public static void AddMetadataCollection(this GetPreSignedUrlRequest request,
            Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }
    }
}