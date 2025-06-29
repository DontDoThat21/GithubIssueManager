using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GitHubIssueManager.Maui.Services;
using System.Security.Claims;

namespace GitHubIssueManager.Maui.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // Simple authentication - in production, validate against Azure AD or other identity provider
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "UserId and Email are required" });
            }

            var token = _authService.GenerateJwtToken(request.UserId, request.Email, request.Roles);
            
            return Ok(new { 
                token,
                expires = DateTime.UtcNow.AddHours(24),
                user = new { id = request.UserId, email = request.Email }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Login failed" });
        }
    }

    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var isValid = _authService.ValidateJwtToken(request.Token);
            
            return Ok(new { valid = isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, new { message = "Token validation failed" });
        }
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "ApiJwt")]
    public IActionResult Logout()
    {
        try
        {
            _authService.ClearJwtToken();
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "Logout failed" });
        }
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "ApiJwt")]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            return Ok(new
            {
                id = userId,
                email = email,
                roles = roles,
                isAuthenticated = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "Failed to get user information" });
        }
    }
}

public class LoginRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[]? Roles { get; set; }
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}