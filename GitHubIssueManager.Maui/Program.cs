using GitHubIssueManager.Maui.Components;
using GitHubIssueManager.Maui.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
