namespace PlanWiseApi.Models
{
    public class UserSearchHistory
    {
        public int Id { get; set; }

        public string UserEmail { get; set; } = "";

        public string Prompt { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}