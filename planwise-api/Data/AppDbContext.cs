using Microsoft.EntityFrameworkCore;
using PlanWiseApi.Models;

namespace PlanWiseApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<EventTemplate> EventTemplates => Set<EventTemplate>();
        public DbSet<SavedPlan> SavedPlans => Set<SavedPlan>();

        public DbSet<UserSearchHistory> UserSearchHistories => Set<UserSearchHistory>();
    }
}