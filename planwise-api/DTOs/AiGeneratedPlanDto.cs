namespace PlanWiseApi.DTOs
{
    public class AiGeneratedPlanDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string BudgetLevel { get; set; } = "";
        public int SuggestedGroupSize { get; set; }
        public string LocationType { get; set; } = "";
        public List<string> FoodIdeas { get; set; } = new();
        public List<string> Activities { get; set; } = new();
        public List<string> Decorations { get; set; } = new();
        public int Score { get; set; }
    }
}
