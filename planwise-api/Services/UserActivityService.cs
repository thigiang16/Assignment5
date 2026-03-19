using Microsoft.EntityFrameworkCore;
using PlanWiseApi.Data;
using PlanWiseApi.Models;

namespace PlanWiseApi.Services
{
    public class UserActivityService
    {
        private readonly AppDbContext _context;

        public UserActivityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveSearchAsync(string email, string prompt)
        {
            var history = new UserSearchHistory
            {
                UserEmail = email,
                Prompt = prompt
            };

            _context.UserSearchHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetRecentSearchesAsync(string email)
        {
            return await _context.UserSearchHistories
                .Where(x => x.UserEmail == email)
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x => x.Prompt)
                .ToListAsync();
        }

        public async Task<List<string>> GetSavedPlansAsync(string email)
        {
            return await _context.SavedPlans
                .Where(x => x.UserEmail == email)
                .Select(x => x.Title)
                .ToListAsync();
        }

        public async Task SavePlanAsync(SavedPlan plan)
        {
            _context.SavedPlans.Add(plan);
            await _context.SaveChangesAsync();
        }
    }
}