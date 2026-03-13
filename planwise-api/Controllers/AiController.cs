using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanWiseApi.Data;
using PlanWiseApi.DTOs;
using PlanWiseApi.Models;
using PlanWiseApi.Services;

namespace PlanWiseApi.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly EmbeddingService _embeddingService;
        private readonly SearchService _searchService;
        private readonly UserActivityService _userActivityService;
        private readonly AppDbContext _context;
        private readonly IAIPlanService _aiPlanService;

        public AiController(
            EmbeddingService embeddingService,
            SearchService searchService,
            UserActivityService userActivityService,
            AppDbContext context,
            IAIPlanService aiPlanService)
        {
            _embeddingService = embeddingService;
            _searchService = searchService;
            _userActivityService = userActivityService;
            _context = context;
            _aiPlanService = aiPlanService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] AiGenerateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is required.");

            var query = request.Prompt.Trim();
            var vector = await _embeddingService.VectorizeTextAsync(query);
            var matches = await _searchService.HybridSearchScoredAsync(query, vector, top: 5);

            if (matches.Count == 0)
                return Ok(new List<AiGeneratedPlanDto>());

            var maxScore = matches.Max(m => m.Score);
            var safeMax = maxScore > 0 ? maxScore : 1;

            var plans = matches
                .Select(match => new AiGeneratedPlanDto
                {
                    Title = match.Document.Title,
                    Description = match.Document.Description,
                    BudgetLevel = match.Document.BudgetLevel,
                    SuggestedGroupSize = match.Document.SuggestedGroupSize,
                    LocationType = match.Document.LocationType,
                    FoodIdeas = ParseIdeas(match.Document.FoodIdeas),
                    Activities = ParseIdeas(match.Document.Activities),
                    Decorations = ParseIdeas(match.Document.Decorations),
                    Score = (int)Math.Round((match.Score / safeMax) * 100)
                })
                .ToList();

            var email = User.Identity?.Name;
            await _userActivityService.SaveSearchAsync(email ?? "", request.Prompt);

            return Ok(plans);
        }

        [HttpGet("recent-searches")]
        public async Task<IActionResult> GetRecentSearches()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var searches = await _userActivityService.GetRecentSearchesAsync(email);
            return Ok(searches);
        }

        [HttpGet("saved-plans")]
        public async Task<IActionResult> GetSavedPlans()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var plans = await _userActivityService.GetSavedPlansAsync(email);
            return Ok(plans);
        }

        [HttpPost("save-plan")]
        public async Task<IActionResult> SavePlan([FromBody] SavePlanRequestDto request)
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required.");

            await _userActivityService.SavePlanAsync(new SavedPlan
            {
                UserEmail = email,
                Title = request.Title.Trim()
            });

            return Ok();
        }

        [HttpPost("/api/plans/{id}/expand")]
        public async Task<IActionResult> ExpandPlan(string id, [FromBody] ExpandPlanRequestDto? request)
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var title = request?.Title?.Trim() ?? "";
            var description = request?.Description?.Trim() ?? "";

            if (int.TryParse(id, out var numericId))
            {
                var savedPlan = await _context.SavedPlans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == numericId && p.UserEmail == email);

                if (savedPlan != null)
                    title = savedPlan.Title;
            }
            else
            {
                var decoded = Uri.UnescapeDataString(id).Trim();

                if (!string.IsNullOrWhiteSpace(decoded))
                    title = decoded;
            }

            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Plan title is required to expand a plan.");

            if (string.IsNullOrWhiteSpace(description))
            {
                var template = await _context.EventTemplates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Title == title);

                description = template?.Description ?? "";
            }

            var suggestions = await _aiPlanService.ExpandPlanAsync(title, description);
            return Ok(suggestions);
        }

        private static List<string> ParseIdeas(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            return raw
                .Split(new[] { '\n', ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
