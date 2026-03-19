using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace PlanWiseApi.Services;

public class ChatService
{
    private readonly PlanWiseApi.Models.Config.AzureOpenAIConfig _config;
    private readonly SearchService _searchService;
    private readonly EmbeddingService _embeddingService;
    private readonly HttpClient _httpClient;

    public ChatService(
        IOptions<PlanWiseApi.Models.Config.AzureOpenAIConfig> config,
        SearchService searchService,
        EmbeddingService embeddingService,
        IHttpClientFactory httpClientFactory)
    {
        _config = config.Value;
        _searchService = searchService;
        _embeddingService = embeddingService;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> AskAsync(string question)
    {
        var vector = await _embeddingService.VectorizeTextAsync(question);
        var results = await _searchService.HybridSearchWithContentAsync(question, vector);

        var context = "No event templates found.";
        if (results.Count > 0)
        {
            var contextBuilder = new StringBuilder();
            foreach (var snippet in results)
            {
                contextBuilder.AppendLine($"Template: {snippet.Title}");
                contextBuilder.AppendLine(snippet.ContentText);
                contextBuilder.AppendLine();
            }

            context = contextBuilder.ToString();
        }

        var instructions = """
        You are an AI assistant that helps users plan events.
        Use the provided event template context first.
        Suggest templates that match the user request.
        If context is empty, still provide practical planning guidance.
        """;

        var payload = JsonSerializer.Serialize(new
        {
            messages = new[]
            {
                new { role = "system", content = instructions },
                new { role = "user", content = $"User request: {question}\n\nTemplate context:\n{context}" }
            },
            temperature = 0.7,
            max_tokens = 600
        });

        var endpoint = _config.Endpoint.TrimEnd('/');
        var url = $"{endpoint}/openai/deployments/{_config.ChatDeployment}/chat/completions?api-version=2024-02-01";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("api-key", _config.ApiKey);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return string.IsNullOrWhiteSpace(content)
            ? "I could not generate a response right now."
            : content;
    }
}

public record DocumentSnippet
{
    public string Title { get; init; } = "";
    public string? ContentText { get; init; }
    public double Score { get; init; }
}