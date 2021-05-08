using System.Collections.Generic;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.UploadAudio;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Xunit;

namespace Audiochan.UnitTests.Validations
{
    public class UploadAudioValidationTests
    {
        private readonly IValidator<UploadAudioRequest> _validator;

        public UploadAudioValidationTests()
        {
            var options = Options.Create(new MediaStorageSettings
            {
                Audio = new MediaStorageSettings.StorageSettings
                {
                    Container = "audios",
                    ValidContentTypes = new List<string>
                    {
                        "audio/mp3",
                        "audio/mpeg",
                        "audio/ogg"
                    },
                    MaximumFileSize = 262144000
                }
            });
            _validator = new UploadAudioRequestValidator(options);
        }
    }
}