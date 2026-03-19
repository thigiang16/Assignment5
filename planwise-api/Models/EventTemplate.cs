namespace PlanWiseApi.Models
{
    public class EventTemplate
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Category { get; set; } = "";

        public string Description { get; set; } = "";

        public string LocationType { get; set; } = ""; 
        
        public int SuggestedGroupSize { get; set; }

        public string FoodIdeas { get; set; } = "";

        public string Activities { get; set; } = "";

        public string Decorations { get; set; } = "";

        public string BudgetLevel { get; set; } = "";

        public string Tags { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}