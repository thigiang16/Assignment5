using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PlanWiseApi.DTOs;
using PlanWiseApi.Models.Config;

namespace PlanWiseApi.Services
{
    public class AIPlanService : IAIPlanService
    {
        private readonly AzureOpenAIConfig _config;
        private readonly HttpClient _httpClient;

        public AIPlanService(IOptions<AzureOpenAIConfig> config, IHttpClientFactory httpClientFactory)
        {
            _config = config.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ExpandPlanSuggestionsDto> ExpandPlanAsync(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(_config.Endpoint)
                || string.IsNullOrWhiteSpace(_config.ApiKey)
                || string.IsNullOrWhiteSpace(_config.ChatDeployment))
            {
                throw new InvalidOperationException("Azure OpenAI configuration is incomplete.");
            }

            var prompt = BuildPrompt(title, description);

            var payload = JsonSerializer.Serialize(new
            {
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = "You are an event planning assistant. Return valid JSON only."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.6,
                max_tokens = 700
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
                .GetString() ?? "";

            return ParseSuggestions(content);
        }

        private static string BuildPrompt(string title, string description)
        {
            return $@"Expand this event plan with practical, specific ideas.

Event Title:
{title}

Event Description:
{description}

Return JSON only in this exact shape:
{{
    ""musicPlaylistIdeas"": [""...""],
    ""timelineSuggestions"": [""...""],
    ""shoppingListEssentials"": [""...""]
}}

Rules:
- Include 5-8 bullet-like items per section.
- Keep items concise and actionable.
- Do not include markdown, prose, or code fences.";
        }

        private static ExpandPlanSuggestionsDto ParseSuggestions(string rawContent)
        {
            var cleaned = rawContent.Trim();

            if (cleaned.StartsWith("```"))
            {
                cleaned = cleaned
                    .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("```", "")
                    .Trim();
            }

            try
            {
                var dto = JsonSerializer.Deserialize<ExpandPlanSuggestionsDto>(
                    cleaned,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (dto != null)
                    return dto;
            }
            catch
            {
                // Fall back to a safe text-based parse below.
            }

            return FallbackParse(cleaned);
        }

        private static ExpandPlanSuggestionsDto FallbackParse(string text)
        {
            var lines = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line => line.TrimStart('-', '*', ' '))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var chunkSize = Math.Max(1, lines.Count / 3);

            return new ExpandPlanSuggestionsDto
            {
                MusicPlaylistIdeas = lines.Take(chunkSize).ToList(),
                TimelineSuggestions = lines.Skip(chunkSize).Take(chunkSize).ToList(),
                ShoppingListEssentials = lines.Skip(chunkSize * 2).ToList()
            };
        }
    }
}
