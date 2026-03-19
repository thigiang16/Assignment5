using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using PlanWiseApi.Models.Config;
using PlanWiseApi.Models.Search;

namespace PlanWiseApi.Services
{
    public class SearchService
    {
        private readonly SearchClient _searchClient;
        private readonly SearchIndexClient _indexClient;
        private readonly string _indexName;

        public SearchService(IOptions<AzureAISearchConfig> config)
        {
            var searchConfig = config.Value;
            var credential = new AzureKeyCredential(searchConfig.AdminApiKey);

            _indexName = searchConfig.IndexName;

            _indexClient = new SearchIndexClient(
                new Uri(searchConfig.Endpoint),
                credential);

            _searchClient = _indexClient.GetSearchClient(_indexName);
        }

        public async Task EnsureIndexAsync()
        {
            var fields = new FieldBuilder().Build(typeof(EventTemplateDocument));

            var index = new SearchIndex(_indexName, fields)
            {
                VectorSearch = new VectorSearch
                {
                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration("hnsw-algorithm")
                    },
                    Profiles =
                    {
                        new VectorSearchProfile("vector-profile","hnsw-algorithm")
                    }
                },
                SemanticSearch = new SemanticSearch
                {
                    Configurations =
                    {
                        new SemanticConfiguration("semantic-config",
                        new SemanticPrioritizedFields
                        {
                            TitleField = new SemanticField("Title"),
                            ContentFields =
                            {
                                new SemanticField("ContentText")
                            }
                        })
                    }
                }
            };

            try
            {
                await _indexClient.GetIndexAsync(_indexName);
                return; // index already exists
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                await _indexClient.CreateIndexAsync(index);
            }
        }

        public async Task UploadTemplateAsync(EventTemplateDocument doc)
        {
            var batch = IndexDocumentsBatch.Upload(new[] { doc });

            await _searchClient.IndexDocumentsAsync(batch);
        }

        public async Task<List<EventTemplateDocument>> HybridSearchAsync(
            string queryText,
            float[] queryVector,
            int top = 5)
        {
            var options = new SearchOptions
            {
                Size = top,

                QueryType = SearchQueryType.Semantic,

                SemanticSearch = new SemanticSearchOptions
                {
                    SemanticConfigurationName = "semantic-config"
                },

                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(queryVector)
                        {
                            KNearestNeighborsCount = top,
                            Fields = { "ContentVector" }
                        }
                    }
                }
            };

            var response =
                await _searchClient.SearchAsync<EventTemplateDocument>(queryText, options);

            var results = new List<EventTemplateDocument>();

            await foreach (var result in response.Value.GetResultsAsync())
            {
                results.Add(result.Document);
            }

            return results;
        }

        public async Task<List<DocumentSnippet>> HybridSearchWithContentAsync(
            string queryText,
            float[] queryVector,
            int top = 5)
        {
            var options = new SearchOptions
            {
                Size = top,
                QueryType = SearchQueryType.Semantic,
                SemanticSearch = new SemanticSearchOptions
                {
                    SemanticConfigurationName = "semantic-config"
                },
                Select = { "Id", "Title", "ContentText", "Category", "Tags" },
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(queryVector)
                        {
                            KNearestNeighborsCount = top,
                            Fields = { "ContentVector" }
                        }
                    }
                }
            };

            var response =
                await _searchClient.SearchAsync<EventTemplateDocument>(queryText, options);

            var results = new List<DocumentSnippet>();

            await foreach (var result in response.Value.GetResultsAsync())
            {
                var doc = result.Document;
                results.Add(new DocumentSnippet
                {
                    Title = doc.Title,
                    ContentText = doc.ContentText,
                    Score = result.Score ?? 0
                });
            }

            return results;
        }

        public async Task<List<ScoredTemplateDocument>> HybridSearchScoredAsync(
            string queryText,
            float[] queryVector,
            int top = 5)
        {
            var options = new SearchOptions
            {
                Size = top,
                QueryType = SearchQueryType.Semantic,
                SemanticSearch = new SemanticSearchOptions
                {
                    SemanticConfigurationName = "semantic-config"
                },
                Select =
                {
                    "Id", "Title", "Description", "LocationType",
                    "SuggestedGroupSize", "BudgetLevel",
                    "FoodIdeas", "Activities", "Decorations"
                },
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(queryVector)
                        {
                            KNearestNeighborsCount = top,
                            Fields = { "ContentVector" }
                        }
                    }
                }
            };

            var response = await _searchClient.SearchAsync<EventTemplateDocument>(queryText, options);
            var results = new List<ScoredTemplateDocument>();

            await foreach (var result in response.Value.GetResultsAsync())
            {
                results.Add(new ScoredTemplateDocument
                {
                    Document = result.Document,
                    Score = result.Score ?? 0
                });
            }

            return results;
        }
    }

    public record ScoredTemplateDocument
    {
        public EventTemplateDocument Document { get; init; } = new();
        public double Score { get; init; }
    }
}