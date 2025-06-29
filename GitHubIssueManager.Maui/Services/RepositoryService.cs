using GitHubIssueManager.Maui.Models;
using System.Text.Json;

namespace GitHubIssueManager.Maui.Services;

public class RepositoryService
{
    private readonly List<GitHubRepository> _watchedRepositories = new();
    private readonly object _lock = new();
    private readonly ILogger<RepositoryService> _logger;
    private readonly string _dataPath;

    public RepositoryService(ILogger<RepositoryService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _dataPath = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataPath);
        LoadWatchedRepositories();
    }

    public event Action? WatchedRepositoriesChanged;

    public IEnumerable<GitHubRepository> GetWatchedRepositories()
    {
        return _watchedRepositories.AsReadOnly();
    }

    public void AddRepository(GitHubRepository repository)
    {
        if (!_watchedRepositories.Any(r => r.Id == repository.Id))
        {
            _watchedRepositories.Add(repository);
            SaveWatchedRepositories();
            WatchedRepositoriesChanged?.Invoke();
            _logger.LogInformation("Added repository to watchlist: {FullName}", repository.FullName);
        }
    }

    public void RemoveRepository(long repositoryId)
    {
        var repository = _watchedRepositories.FirstOrDefault(r => r.Id == repositoryId);
        if (repository != null)
        {
            _watchedRepositories.Remove(repository);
            SaveWatchedRepositories();
            WatchedRepositoriesChanged?.Invoke();
            _logger.LogInformation("Removed repository from watchlist: {FullName}", repository.FullName);
        }
    }

    public bool IsWatched(long repositoryId)
    {
        return _watchedRepositories.Any(r => r.Id == repositoryId);
    }

    private void LoadWatchedRepositories()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "watched-repositories.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(json);
                if (repositories != null)
                {
                    _watchedRepositories.AddRange(repositories);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading watched repositories");
        }
    }

    private void SaveWatchedRepositories()
    {
        try
        {
            var filePath = Path.Combine(_dataPath, "watched-repositories.json");
            var json = JsonSerializer.Serialize(_watchedRepositories, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving watched repositories");
        }
    }
}