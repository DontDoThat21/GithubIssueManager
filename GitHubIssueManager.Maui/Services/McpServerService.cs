using GitHubIssueManager.Maui.Services;

namespace GitHubIssueManager.Maui.Services;

public class McpServerService
{
    private readonly GitHubService _gitHubService;
    private readonly RepositoryService _repositoryService;
    private readonly AuthenticationService _authenticationService;
    private readonly ILogger<McpServerService> _logger;

    public McpServerService(
        GitHubService gitHubService,
        RepositoryService repositoryService,
        AuthenticationService authenticationService,
        ILogger<McpServerService> logger)
    {
        _gitHubService = gitHubService;
        _repositoryService = repositoryService;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MCP Server service starting...");
        
        // Check if authentication is properly configured
        if (!_authenticationService.IsApiAuthenticated)
        {
            _logger.LogWarning("MCP Server starting without API authentication configured");
        }
        else
        {
            _logger.LogInformation("MCP Server starting with authentication enabled");
        }
        
        // TODO: Implement MCP server protocol
        // This will be implemented in Phase 3
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MCP Server service stopping...");
        await Task.CompletedTask;
    }

    public bool IsAuthenticationRequired => true;
    
    public bool ValidateApiAccess(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("API access attempted without token");
            return false;
        }

        var isValid = _authenticationService.ValidateJwtToken(token);
        if (!isValid)
        {
            _logger.LogWarning("API access attempted with invalid token");
        }
        
        return isValid;
    }
}