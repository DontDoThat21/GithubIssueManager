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

// Add Microsoft Authentication (optional - for future Azure AD integration)
// Only configure if ClientId is provided in configuration
var azureAdSection = builder.Configuration.GetSection("AzureAd");
if (!string.IsNullOrEmpty(azureAdSection["ClientId"]))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(azureAdSection)
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddInMemoryTokenCaches();
}
else
{
    // Configure basic JWT authentication for MCP server
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
}

// Add JWT Bearer authentication for API endpoints
var jwtSettings = builder.Configuration.GetSection("Authentication:Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "your-secret-key-here-must-be-at-least-32-characters-long");

builder.Services.AddAuthentication()
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

// Add GitHub Issue Manager services
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<GitHubService>();
builder.Services.AddSingleton<RepositoryService>();
builder.Services.AddSingleton<McpServerService>();

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
