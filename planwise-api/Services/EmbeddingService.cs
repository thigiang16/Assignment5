using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PlanWiseApi.Models;

namespace PlanWiseApi.Services
{
    public class EmbeddingService
    {
        private readonly PlanWiseApi.Models.Config.AzureOpenAIConfig _config;
        private readonly HttpClient _httpClient;

        public EmbeddingService(IOptions<PlanWiseApi.Models.Config.AzureOpenAIConfig> config, IHttpClientFactory httpClientFactory)
        {
            _config = config.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<float[]> VectorizeTextAsync(string text)
        {
            var endpoint = _config.Endpoint.TrimEnd('/');
            var url = $"{endpoint}/openai/deployments/{_config.EmbeddingDeployment}/embeddings?api-version=2024-02-01";

            var payload = JsonSerializer.Serialize(new
            {
                input = text
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("api-key", _config.ApiKey);

            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var vectorElement = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding");

            var vector = new float[vectorElement.GetArrayLength()];

            int i = 0;
            foreach (var element in vectorElement.EnumerateArray())
            {
                vector[i++] = element.GetSingle();
            }

            return vector;
        }

        public async Task<float[]> VectorizeEventTemplateAsync(EventTemplate template)
        {
            var combinedText = $"""
            Event Title: {template.Title}
            Category: {template.Category}
            Description: {template.Description}
            Location Type: {template.LocationType}
            Suggested Group Size: {template.SuggestedGroupSize}

            Food Ideas:
            {template.FoodIdeas}

            Activities:
            {template.Activities}

            Decorations:
            {template.Decorations}

            Budget Level: {template.BudgetLevel}

            Tags:
            {template.Tags}
            """;

            return await VectorizeTextAsync(combinedText);
        }
    }
}