using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.UploadAudio;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Audiochan.IntegrationTests.Features.Audios
{
    [Collection(nameof(SliceFixture))]
    public class CreateAudioRequestTests
    {
        private readonly SliceFixture _fixture;
        private readonly Randomizer _randomizer;

        public CreateAudioRequestTests(SliceFixture fixture)
        {
            _fixture = fixture;
            _randomizer = new Randomizer();
        }
    }
}