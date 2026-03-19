using PlanWiseApi.Models;
using PlanWiseApi.Models.Search;

namespace PlanWiseApi.Services
{
    public class TemplateIndexService
    {
        private readonly SearchService _searchService;
        private readonly EmbeddingService _embeddingService;

        public TemplateIndexService(
            SearchService searchService,
            EmbeddingService embeddingService)
        {
            _searchService = searchService;
            _embeddingService = embeddingService;
        }

        public async Task IndexTemplateAsync(EventTemplate template)
        {
            var content =
                $"{template.Title} {template.Description} {template.Tags}";

            var vector =
                await _embeddingService.VectorizeTextAsync(content);

            var doc = new EventTemplateDocument
            {
                Id = template.Id.ToString(),
                Title = template.Title,
                Category = template.Category,
                Description = template.Description,
                LocationType = template.LocationType,
                SuggestedGroupSize = template.SuggestedGroupSize,
                FoodIdeas = template.FoodIdeas,
                Activities = template.Activities,
                Decorations = template.Decorations,
                BudgetLevel = template.BudgetLevel,
                Tags = template.Tags,
                ContentText = content,
                ContentVector = vector
            };
            await _searchService.UploadTemplateAsync(doc);
        }
    }
}