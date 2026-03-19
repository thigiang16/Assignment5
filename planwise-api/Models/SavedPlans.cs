namespace PlanWiseApi.Models
{
    public class SavedPlan
    {
        public int Id { get; set; }

        public string UserEmail { get; set; } = "";

        public string Title { get; set; } = "";
    }
}