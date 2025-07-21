using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GitHubIssueManager.Maui.Services;
using GithubIssueManager.Services;

namespace GitHubIssueManager.Maui.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "ApiJwt")]
public class McpController : ControllerBase
{
    private readonly McpServerService _mcpService;
    private readonly GitHubService _gitHubService;
    private readonly AuthenticationService _authService;
    private readonly ILogger<McpController> _logger;

    public McpController(
        McpServerService mcpService,
        GitHubService gitHubService,
        AuthenticationService authService,
        ILogger<McpController> logger)
    {
        _mcpService = mcpService;
        _gitHubService = gitHubService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        try
        {
            return Ok(new
            {
                serverStatus = "running",
                authenticationRequired = _mcpService.IsAuthenticationRequired,
                githubAuthenticated = _authService.IsAuthenticated,
                apiAuthenticated = _authService.IsApiAuthenticated,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MCP status");
            return StatusCode(500, new { message = "Failed to get server status" });
        }
    }

    [HttpGet("capabilities")]
    public IActionResult GetCapabilities()
    {
        try
        {
            return Ok(new
            {
                version = "1.0.0",
                capabilities = new[]
                {
                    "github_repository_management",
                    "github_issue_management",
                    "authentication_required",
                    "jwt_token_support"
                },
                authentication = new
                {
                    type = "JWT",
                    required = true,
                    schemes = new[] { "Bearer" }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting MCP capabilities");
            return StatusCode(500, new { message = "Failed to get capabilities" });
        }
    }

    [HttpPost("validate-access")]
    public IActionResult ValidateAccess([FromHeader] string? authorization)
    {
        try
        {
            var token = authorization?.Replace("Bearer ", "");
            var isValid = _mcpService.ValidateApiAccess(token);
            
            return Ok(new 
            { 
                valid = isValid,
                message = isValid ? "Access granted" : "Access denied - invalid or missing token"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating MCP access");
            return StatusCode(500, new { message = "Access validation failed" });
        }
    }
}