namespace GitHubIssueManager.Maui.Services;

public class McpServerService
{
    private readonly GitHubService _gitHubService;
    private readonly RepositoryService _repositoryService;
    private readonly ILogger<McpServerService> _logger;

    public McpServerService(
        GitHubService gitHubService,
        RepositoryService repositoryService,
        ILogger<McpServerService> logger)
    {
        _gitHubService = gitHubService;
        _repositoryService = repositoryService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MCP Server service starting...");
        // TODO: Implement MCP server protocol
        // This will be implemented in Phase 3
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MCP Server service stopping...");
        await Task.CompletedTask;
    }
}