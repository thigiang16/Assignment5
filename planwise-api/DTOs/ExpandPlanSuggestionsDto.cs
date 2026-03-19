namespace PlanWiseApi.DTOs
{
    public class ExpandPlanSuggestionsDto
    {
        public List<string> MusicPlaylistIdeas { get; set; } = new();
        public List<string> TimelineSuggestions { get; set; } = new();
        public List<string> ShoppingListEssentials { get; set; } = new();
    }
}
