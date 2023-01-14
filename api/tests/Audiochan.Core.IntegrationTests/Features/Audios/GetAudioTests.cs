using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Audiochan.Core.Dtos;
using Audiochan.Core.Helpers;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Tests.Common.Fakers.Audios;
using Audiochan.Tests.Common.Fakers.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Audiochan.Core.IntegrationTests.Features.Audios;

[Collection(nameof(SharedTestCollection))]
public class GetAudioTests
{
    private readonly AudiochanApiFactory _factory;
    private readonly HttpClient _httpClient;

    public GetAudioTests(AudiochanApiFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task ShouldNotGetAudio_WhenAudioIdIsInvalid()
    {
        // Assign
        using var scope = _factory.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = new UserFaker().Generate();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        var audio = new AudioFaker(user.Id).Generate();
        await dbContext.Audios.AddAsync(audio);
        await dbContext.SaveChangesAsync();

        // Act
        var invalidSlug = HashIdHelper.EncodeLong(0);
        using var response = await _httpClient.GetAsync($"audios/${invalidSlug}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
        
    [Fact]
    public async Task ShouldSuccessfullyGetAudio()
    {
        // Assign
        using var scope = _factory.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var user = new UserFaker().Generate();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        
        var audio = new AudioFaker(user.Id).Generate();
        await dbContext.Audios.AddAsync(audio);
        await dbContext.SaveChangesAsync();

        // Act
        var slug = HashIdHelper.EncodeLong(audio.Id);
        using var response = await _httpClient.GetAsync($"audios/${slug}");
        var result = await response.Content.ReadFromJsonAsync<AudioDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeOfType<AudioDto>();
        result!.Title.Should().Be(audio.Title);
        result.Description.Should().Be(audio.Description);
        result.Created.Should().Be(audio.Created);
        result.Duration.Should().Be(audio.Duration);
        result.Picture.Should().BeNullOrEmpty();
        result.Tags.Count.Should().Be(audio.Tags.Count);
        result.Src.Should().Be(audio.File);
        result.Size.Should().Be(audio.Size);
        result.LastModified.Should().BeNull();
        result.User.Should().NotBeNull();
        result.User.Should().BeOfType<UserDto>();
        result.User.Id.Should().Be(user.Id);
    }
}