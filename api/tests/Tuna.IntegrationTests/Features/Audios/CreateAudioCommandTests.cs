using Moq;
using Tuna.Application.Services;

namespace Tuna.IntegrationTests.Features.Audios;

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

    // [Fact]
    // public async Task Should_Create_New_Audio()
    // {
    //     var faker = new Faker();
    //     var (user, principal) = await _fixture.CreateRandomUserAsync(faker);
    //     await using var result = await _fixture.ExecuteGraphQlRequestAsync(builder =>
    //     {
    //         builder
    //             .SetGlobalState(nameof(ClaimsPrincipal), principal)
    //             .SetVariableValue("uploadId", "test")
    //             .SetVariableValue("title", faker.Lorem.Sentence())
    //             .SetVariableValue("description", faker.Lorem.Paragraph())
    //             .SetVariableValue("fileName", faker.System.FileName("mp3"))
    //             .SetVariableValue("fileSize", faker.Random.Long(1000, 1000000))
    //             .SetVariableValue("duration", faker.Random.Decimal(1000, 1000000))
    //             .SetQuery(
    //                 @"mutation CreateAudio($uploadId: String!, $title: String!, $description: String, $fileName: String!, $fileSize: Long!, $duration: Decimal!) {
    //                     createAudio(input: {
    //                         uploadId: $uploadId,
    //                         title: $title,
    //                         description: $description,
    //                         fileName: $fileName,
    //                         fileSize: $fileSize,
    //                         duration: $duration
    //                     }) {
    //                         audio {
    //                             id
    //                             title
    //                             description
    //                             fileName
    //                             fileSize
    //                             duration
    //                             createdAt
    //                             updatedAt
    //                             user {
    //                                 id
    //                                 username
    //                                 avatarUrl
    //                             }
    //                         }
    //                     }
    //                 }");
    //     });
    //
    //     var queryResult = result.ExpectQueryResult();
    //     
    //     
    // }
}