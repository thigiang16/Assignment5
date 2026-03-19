using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using PlanWiseApi.Models;
using PlanWiseApi.Models.Config;
using PlanWiseApi.Services;
using PlanWiseApi.Tests.TestDoubles;

namespace PlanWiseApi.Tests.Services;

public class EmbeddingServiceTests
{
    [Fact]
    public async Task VectorizeTextAsync_ValidResponse_ReturnsVector()
    {
        const string responseBody = """
        {
          "data": [
            {
              "embedding": [0.1, 0.2, 0.3]
            }
          ]
        }
        """;

        var service = CreateService(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, responseBody));

        var vector = await service.VectorizeTextAsync("hello");

        Assert.Equal(3, vector.Length);
        Assert.Equal(0.1f, vector[0], 3);
        Assert.Equal(0.2f, vector[1], 3);
        Assert.Equal(0.3f, vector[2], 3);
    }

    [Fact]
    public async Task VectorizeTextAsync_HttpFailure_ThrowsHttpRequestException()
    {
        var service = CreateService(_ => StubHttpMessageHandler.Json(HttpStatusCode.BadRequest, "{}"));

        await Assert.ThrowsAsync<HttpRequestException>(() => service.VectorizeTextAsync("bad"));
    }

    [Fact]
    public async Task VectorizeEventTemplateAsync_ValidTemplate_ReturnsVector()
    {
        const string responseBody = """
        {
          "data": [
            {
              "embedding": [1.0, 2.0]
            }
          ]
        }
        """;

        var service = CreateService(_ => StubHttpMessageHandler.Json(HttpStatusCode.OK, responseBody));

        var template = new EventTemplate
        {
            Title = "Birthday",
            Category = "Party",
            Description = "Indoor celebration",
            LocationType = "Indoor",
            SuggestedGroupSize = 20,
            FoodIdeas = "Cake",
            Activities = "Games",
            Decorations = "Balloons",
            BudgetLevel = "Medium",
            Tags = "fun"
        };

        var vector = await service.VectorizeEventTemplateAsync(template);

        Assert.Equal(2, vector.Length);
        Assert.Equal(1.0f, vector[0], 3);
        Assert.Equal(2.0f, vector[1], 3);
    }

    private static EmbeddingService CreateService(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        var config = Options.Create(new AzureOpenAIConfig
        {
            Endpoint = "https://example.openai.azure.com/",
            ApiKey = "test-key",
            EmbeddingDeployment = "embedding-model"
        });

        var httpClient = new HttpClient(new StubHttpMessageHandler(responseFactory));
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        return new EmbeddingService(config, factory.Object);
    }
}
