using PlanWiseApi.Models.Auth;

namespace PlanWiseApi.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);

        Task<bool> RegisterAsync(RegisterDto dto);
    }
}