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
    }
}