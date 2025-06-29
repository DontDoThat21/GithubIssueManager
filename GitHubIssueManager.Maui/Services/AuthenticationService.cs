using System.Text.Json;

namespace GitHubIssueManager.Maui.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly string _dataPath;
    private string? _currentToken;

    public AuthenticationService(ILogger<AuthenticationService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _dataPath = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataPath);
        LoadStoredToken();
    }

    public event Action<bool>? AuthenticationStateChanged;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);

    public string? CurrentToken => _currentToken;

    public void SetToken(string token)
    {
        _currentToken = token;
        SaveToken();
        AuthenticationStateChanged?.Invoke(IsAuthenticated);
        _logger.LogInformation("GitHub token configured");
    }

    public void ClearToken()
    {
        _currentToken = null;
        DeleteStoredToken();
        AuthenticationStateChanged?.Invoke(IsAuthenticated);
        _logger.LogInformation("GitHub token cleared");
    }

    private void LoadStoredToken()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "auth.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var authData = JsonSerializer.Deserialize<AuthData>(json);
                _currentToken = authData?.Token;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stored token");
        }
    }

    private void SaveToken()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "auth.json");
            var authData = new AuthData { Token = _currentToken };
            var json = JsonSerializer.Serialize(authData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving token");
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
            _logger.LogError(ex, "Error deleting stored token");
        }
    }

    private class AuthData
    {
        public string? Token { get; set; }
    }
}