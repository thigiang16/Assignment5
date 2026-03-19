using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using PlanWiseApi.Models.Config;
using PlanWiseApi.Services;
using PlanWiseApi.Tests.TestDoubles;

namespace PlanWiseApi.Tests.Services;

public class AIPlanServiceTests
{
    [Fact]
    public async Task ExpandPlanAsync_MissingConfiguration_ThrowsInvalidOperationException()
    {
        var options = Options.Create(new AzureOpenAIConfig());
        var httpFactory = CreateHttpClientFactory(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, "{}"));
        var service = new AIPlanService(options, httpFactory.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ExpandPlanAsync("Party", "Desc"));
    }

    [Fact]
    public async Task ExpandPlanAsync_ValidJsonResponse_ReturnsStructuredSuggestions()
    {
        const string modelContent = """
        {
          "musicPlaylistIdeas": ["Pop classics", "Lo-fi lounge"],
          "timelineSuggestions": ["6:00 PM welcome", "7:00 PM dinner"],
          "shoppingListEssentials": ["Plates", "Napkins"]
        }
        """;

        var responseBody = BuildChatCompletionsResponse(modelContent);

        var options = Options.Create(CreateValidOpenAiConfig());
        var httpFactory = CreateHttpClientFactory(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, responseBody));
        var service = new AIPlanService(options, httpFactory.Object);

        var result = await service.ExpandPlanAsync("Party", "Fun party");

        Assert.Equal(2, result.MusicPlaylistIdeas.Count);
        Assert.Equal("Pop classics", result.MusicPlaylistIdeas[0]);
        Assert.Equal(2, result.TimelineSuggestions.Count);
        Assert.Equal(2, result.ShoppingListEssentials.Count);
    }

    [Fact]
    public async Task ExpandPlanAsync_CodeFenceJsonResponse_ParsesSuggestions()
    {
        const string modelContent = """
        ```json
        {
          "musicPlaylistIdeas": ["Jazz"],
          "timelineSuggestions": ["6 PM setup"],
          "shoppingListEssentials": ["Ice"]
        }
        ```
        """;

        var responseBody = BuildChatCompletionsResponse(modelContent);

        var options = Options.Create(CreateValidOpenAiConfig());
        var httpFactory = CreateHttpClientFactory(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, responseBody));
        var service = new AIPlanService(options, httpFactory.Object);

        var result = await service.ExpandPlanAsync("Party", "Fun party");

        Assert.Single(result.MusicPlaylistIdeas);
        Assert.Equal("Jazz", result.MusicPlaylistIdeas[0]);
        Assert.Single(result.TimelineSuggestions);
        Assert.Single(result.ShoppingListEssentials);
    }

    [Fact]
    public async Task ExpandPlanAsync_NonJsonResponse_FallbackParsesIntoSections()
    {
        const string modelContent = """
        - Track 1
        - Track 2
        - 6 PM welcome
        - 7 PM food
        - Paper cups
        - Candles
        """;

        var responseBody = BuildChatCompletionsResponse(modelContent);

        var options = Options.Create(CreateValidOpenAiConfig());
        var httpFactory = CreateHttpClientFactory(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, responseBody));
        var service = new AIPlanService(options, httpFactory.Object);

        var result = await service.ExpandPlanAsync("Party", "Fun party");

        Assert.NotEmpty(result.MusicPlaylistIdeas);
        Assert.NotEmpty(result.TimelineSuggestions);
        Assert.NotEmpty(result.ShoppingListEssentials);
    }

    private static AzureOpenAIConfig CreateValidOpenAiConfig()
    {
        return new AzureOpenAIConfig
        {
            Endpoint = "https://example.openai.azure.com/",
            ApiKey = "test-key",
            ChatDeployment = "chat-model"
        };
    }

    private static string BuildChatCompletionsResponse(string modelContent)
    {
        return $$"""
        {
          "choices": [
            {
              "message": {
                "content": {{System.Text.Json.JsonSerializer.Serialize(modelContent)}}
              }
            }
          ]
        }
        """;
    }

    private static Mock<IHttpClientFactory> CreateHttpClientFactory(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var httpClient = new HttpClient(new StubHttpMessageHandler(responseFactory));
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        return factory;
    }
}
