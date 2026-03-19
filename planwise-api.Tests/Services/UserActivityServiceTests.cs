using Microsoft.EntityFrameworkCore;
using PlanWiseApi.Data;
using PlanWiseApi.Models;
using PlanWiseApi.Services;

namespace PlanWiseApi.Tests.Services;

public class UserActivityServiceTests
{
    [Fact]
    public async Task SaveSearchAsync_ValidInput_PersistsHistory()
    {
        using var context = CreateContext();
        var service = new UserActivityService(context);

        await service.SaveSearchAsync("user@example.com", "birthday party");

        var history = await context.UserSearchHistories.SingleAsync();
        Assert.Equal("user@example.com", history.UserEmail);
        Assert.Equal("birthday party", history.Prompt);
    }

    [Fact]
    public async Task GetRecentSearchesAsync_MoreThanFiveSearches_ReturnsLatestFive()
    {
        using var context = CreateContext();

        for (var i = 1; i <= 7; i++)
        {
            context.UserSearchHistories.Add(new UserSearchHistory
            {
                UserEmail = "user@example.com",
                Prompt = $"search-{i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(i)
            });
        }

        await context.SaveChangesAsync();

        var service = new UserActivityService(context);
        var result = await service.GetRecentSearchesAsync("user@example.com");

        Assert.Equal(5, result.Count);
        Assert.Equal("search-7", result[0]);
        Assert.Equal("search-3", result[4]);
    }

    [Fact]
    public async Task GetSavedPlansAsync_UserHasPlans_ReturnsOnlyUserTitles()
    {
        using var context = CreateContext();

        context.SavedPlans.AddRange(
            new SavedPlan { UserEmail = "user@example.com", Title = "Plan A" },
            new SavedPlan { UserEmail = "user@example.com", Title = "Plan B" },
            new SavedPlan { UserEmail = "other@example.com", Title = "Other Plan" }
        );

        await context.SaveChangesAsync();

        var service = new UserActivityService(context);
        var result = await service.GetSavedPlansAsync("user@example.com");

        Assert.Equal(2, result.Count);
        Assert.Contains("Plan A", result);
        Assert.Contains("Plan B", result);
        Assert.DoesNotContain("Other Plan", result);
    }

    [Fact]
    public async Task SavePlanAsync_ValidPlan_PersistsPlan()
    {
        using var context = CreateContext();
        var service = new UserActivityService(context);

        await service.SavePlanAsync(new SavedPlan
        {
            UserEmail = "user@example.com",
            Title = "Plan X"
        });

        var plan = await context.SavedPlans.SingleAsync();
        Assert.Equal("user@example.com", plan.UserEmail);
        Assert.Equal("Plan X", plan.Title);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
