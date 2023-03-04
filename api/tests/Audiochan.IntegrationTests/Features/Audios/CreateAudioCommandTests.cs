using Audiochan.Core;
using Audiochan.Core.Features.Audios.Commands;
using Audiochan.Core.Persistence;
using Audiochan.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Audiochan.IntegrationTests.Features.Audios;

[Collection(nameof(TestFixture))]
public class CreateAudioCommandTests
{
    private readonly TestFixture _fixture;
    private readonly Mock<IStorageService> _storageServiceMock;

    public CreateAudioCommandTests(TestFixture fixture, Mock<IStorageService> storageServiceMock)
    {
        _fixture = fixture;
        _storageServiceMock = storageServiceMock;
    }

    [Fact]
    public async Task Should_Create_New_Audio()
    {
        
    }
}