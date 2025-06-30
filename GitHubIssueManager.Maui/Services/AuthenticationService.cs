using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GitHubIssueManager.Maui.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _dataPath;
    private string? _currentToken;
    private string? _currentJwtToken;
    private readonly string SecretKey;

    public AuthenticationService(ILogger<AuthenticationService> logger, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _dataPath = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataPath);
        LoadStoredTokens();
    }

    public event Action<bool>? AuthenticationStateChanged;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
    
    public bool IsApiAuthenticated => !string.IsNullOrEmpty(_currentJwtToken);

    public string? CurrentToken => _currentToken;
    
    public string? CurrentJwtToken => _currentJwtToken;

    public void SetToken(string token)
    {
        _currentToken = token;
        SaveTokens();
        AuthenticationStateChanged?.Invoke(IsAuthenticated);
        _logger.LogInformation("GitHub token configured");
    }

    public void ClearToken()
    {
        _currentToken = null;
        SaveTokens();
        AuthenticationStateChanged?.Invoke(IsAuthenticated);
        _logger.LogInformation("GitHub token cleared");
    }

    public string GenerateJwtToken(string userId, string email, IEnumerable<string>? roles = null)
    {
        var jwtSettings = _configuration.GetSection("Authentication:Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        
        _currentJwtToken = jwtToken;
        SaveTokens();
        
        _logger.LogInformation("JWT token generated for user {UserId}", userId);
        return jwtToken;
    }

    public bool ValidateJwtToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Authentication:Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "your-secret-key-here-must-be-at-least-32-characters-long");
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "JWT token validation failed");
            return false;
        }
    }

    public void ClearJwtToken()
    {
        _currentJwtToken = null;
        SaveTokens();
        _logger.LogInformation("JWT token cleared");
    }

    private void LoadStoredTokens()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "auth.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var authData = JsonSerializer.Deserialize<AuthData>(json);
                _currentToken = authData?.Token;
                _currentJwtToken = authData?.JwtToken;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stored tokens");
        }
    }

    private void SaveTokens()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "auth.json");
            var authData = new AuthData 
            { 
                Token = _currentToken,
                JwtToken = _currentJwtToken
            };
            var json = JsonSerializer.Serialize(authData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving tokens");
        }
    }

    private void DeleteStoredToken()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "auth.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting stored tokens");
        }
    }

    private class AuthData
    {
        public string? Token { get; set; }
        public string? JwtToken { get; set; }
    }
}