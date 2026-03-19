using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlanWiseApi.Data;
using PlanWiseApi.Models;
using PlanWiseApi.Models.Config;
using PlanWiseApi.Services;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "PlanWiseFrontend";
const string LocalFrontendOrigin = "http://localhost:5173";
const string ProductionFrontendOrigin = "https://gentle-glacier-0780e230f.6.azurestaticapps.net";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Select(origin => origin.Trim().TrimEnd('/'))
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? new[] { LocalFrontendOrigin, ProductionFrontendOrigin };

// Local fallback ports for Development when ASPNETCORE_URLS is not provided.
if (builder.Environment.IsDevelopment()
    && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConnection))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(defaultConnection));

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = builder.Configuration["Jwt:Key"];

    if (string.IsNullOrWhiteSpace(key))
    {
        throw new InvalidOperationException("Jwt:Key is missing.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Email,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key!)
        )
    };
});

builder.Services.AddAuthorization();

// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName,
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Azure Configurations
builder.Services.Configure<AzureOpenAIConfig>(
    builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.Configure<AzureAISearchConfig>(
    builder.Configuration.GetSection("AzureAISearch"));

// HttpClient for AI services
builder.Services.AddHttpClient();

// AI Services
builder.Services.AddScoped<EmbeddingService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<TemplateIndexService>();
builder.Services.AddScoped<UserActivityService>();
builder.Services.AddScoped<IAIPlanService, AIPlanService>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")))
{
    app.UseHttpsRedirection();
}

// CORS
app.UseRouting();
app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    environment = app.Environment.EnvironmentName,
    timestampUtc = DateTime.UtcNow
}));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    try
    {
        await db.Database.MigrateAsync();

        var adminExists = await db.Users.AnyAsync(u => u.Email == "admin@planwise.com");

        if (!adminExists)
        {
            db.Users.Add(new User
            {
                Name = "Admin",
                Email = "admin@planwise.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
            logger.LogInformation("Seeded default admin user.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed during database migration or seeding.");
        throw;
    }
}

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");
    var searchConfig = scope.ServiceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<AzureAISearchConfig>>()
        .Value;

    var hasSearchConfig = !string.IsNullOrWhiteSpace(searchConfig.Endpoint)
        && !string.IsNullOrWhiteSpace(searchConfig.AdminApiKey)
        && !string.IsNullOrWhiteSpace(searchConfig.IndexName);

    if (!hasSearchConfig)
    {
        logger.LogWarning("Skipping Azure AI Search index initialization because configuration is incomplete.");
    }
    else
    {
        var searchService = scope.ServiceProvider.GetRequiredService<SearchService>();

        try
        {
            await searchService.EnsureIndexAsync();
            logger.LogInformation("Azure AI Search index is ready.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure AI Search index initialization failed.");
        }
    }
}

app.Run();