using System;
using Audiochan.Core.Helpers;

namespace Audiochan.UnitTests.Mocks
{
    public interface IUploadHelperMock
    {
        string GenerateUploadId();
    }
    
    public class UploadHelperMock : IUploadHelperMock
    {
        public string GenerateUploadId()
        {
            return UploadHelpers.GenerateUploadId();
        }
    }
}