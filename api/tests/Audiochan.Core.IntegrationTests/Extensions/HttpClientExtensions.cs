using System.Net.Http;

namespace Audiochan.Core.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient AddUserIdToAuthorization(this HttpClient httpClient, long userId)
    {
        httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());
        return httpClient;
    }
}