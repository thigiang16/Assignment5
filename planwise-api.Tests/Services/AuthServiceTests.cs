using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlanWiseApi.Data;
using PlanWiseApi.Models;
using PlanWiseApi.Models.Auth;
using PlanWiseApi.Services;

namespace PlanWiseApi.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_EmailDoesNotExist_ReturnsTrueAndPersistsUser()
    {
        using var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var result = await service.RegisterAsync(new RegisterDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        });

        Assert.True(result);

        var user = await context.Users.SingleAsync();
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("User", user.Role);
        Assert.NotEqual("Password123!", user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsFalse()
    {
        using var context = CreateContext();
        context.Users.Add(new User
        {
            Name = "Existing",
            Email = "existing@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new AuthService(context, CreateConfiguration());

        var result = await service.RegisterAsync(new RegisterDto
        {
            Name = "Another",
            Email = "existing@example.com",
            Password = "Password123!"
        });

        Assert.False(result);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
    {
        using var context = CreateContext();
        var password = "Password123!";

        context.Users.Add(new User
        {
            Id = 12,
            Name = "Admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new AuthService(context, CreateConfiguration());

        var token = await service.LoginAsync(new LoginDto
        {
            Email = "admin@example.com",
            Password = password
        });

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Email && c.Value == "admin@example.com");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "12");
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsNull()
    {
        using var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var token = await service.LoginAsync(new LoginDto
        {
            Email = "missing@example.com",
            Password = "Password123!"
        });

        Assert.Null(token);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        using var context = CreateContext();
        context.Users.Add(new User
        {
            Name = "Test",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new AuthService(context, CreateConfiguration());

        var token = await service.LoginAsync(new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        });

        Assert.Null(token);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
            })
            .Build();
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
