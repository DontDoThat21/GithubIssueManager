using GitHubIssueManager.Maui.Components;
using GitHubIssueManager.Maui.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controllers for API endpoints
builder.Services.AddControllers();

// 🆓 Authentication Setup - No external services required!
// 
// This section configures authentication. Currently set up for FREE local JWT authentication.
// Azure AD integration is optional and only activates if ClientId is configured.

// Check for optional Azure AD configuration (enterprise feature)
var azureAdSection = builder.Configuration.GetSection("AzureAd");
if (!string.IsNullOrEmpty(azureAdSection["ClientId"]))
{
    // Azure AD mode (optional - only if ClientId is configured)
    // Note: This requires Azure AD setup but has free tier available
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(azureAdSection)
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches();
}
else
{
    // 🎉 FREE Local Authentication Mode (Current Setup)
    // No external dependencies, no subscriptions required!
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
}

// 🔐 Local JWT Bearer Authentication (FREE)
// This handles token generation and validation completely locally
var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "your-secret-key-here-must-be-at-least-32-characters-long");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("ApiJwt", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configure HttpClient for GitHub API with proper lifetime management
builder.Services.AddHttpClient<GitHubService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(2); // Reasonable timeout for GitHub API calls
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Manage connection pooling

// Add GitHub Issue Manager services
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddScoped<GitHubService>(); // Changed from Singleton to Scoped for proper HttpClient usage
builder.Services.AddSingleton<RepositoryService>();
builder.Services.AddScoped<McpServerService>(); // Changed from Singleton to Scoped to resolve DI error
builder.Services.AddSingleton<IssueFilterService>(); // Add the new filter service
builder.Services.AddScoped<IssueExportService>(); // Add the new export service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API controllers
app.MapControllers();

app.Run();