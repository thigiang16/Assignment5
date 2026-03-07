using System.ComponentModel.DataAnnotations;

namespace PlanWiseApi.Models.Auth
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}