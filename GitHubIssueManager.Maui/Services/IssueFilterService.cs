using GitHubIssueManager.Maui.Models;
using System.Text.Json;

namespace GitHubIssueManager.Maui.Services;

/// <summary>
/// Service for managing issue filters and search operations
/// </summary>
public class IssueFilterService
{
    private readonly ILogger<IssueFilterService> _logger;
    private readonly string _savedFiltersPath;
    private List<SavedFilter> _savedFilters = new();

    /// <summary>
    /// Current active filter
    /// </summary>
    public IssueFilter CurrentFilter { get; private set; } = new();

    /// <summary>
    /// Event fired when filter changes
    /// </summary>
    public event Action<IssueFilter>? FilterChanged;

    public IssueFilterService(ILogger<IssueFilterService> logger)
    {
        _logger = logger;
        _savedFiltersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "GitHubIssueManager", "saved-filters.json");
        
        LoadSavedFilters();
    }

    /// <summary>
    /// Update the current filter and notify subscribers
    /// </summary>
    public void UpdateFilter(IssueFilter filter)
    {
        CurrentFilter = filter.Clone();
        FilterChanged?.Invoke(CurrentFilter);
    }

    /// <summary>
    /// Reset the current filter to default values
    /// </summary>
    public void ResetFilter()
    {
        CurrentFilter.Reset();
        FilterChanged?.Invoke(CurrentFilter);
    }

    /// <summary>
    /// Apply client-side filtering to a list of issues
    /// </summary>
    public IEnumerable<GitHubIssue> ApplyFilter(IEnumerable<GitHubIssue> issues, IssueFilter? filter = null)
    {
        var activeFilter = filter ?? CurrentFilter;
        var filtered = issues.AsEnumerable();

        // Apply search query filter
        if (!string.IsNullOrWhiteSpace(activeFilter.SearchQuery))
        {
            var query = activeFilter.SearchQuery.ToLowerInvariant();
            filtered = filtered.Where(issue =>
                issue.Title.ToLowerInvariant().Contains(query) ||
                (issue.Body?.ToLowerInvariant().Contains(query) ?? false) ||
                issue.User?.Login.ToLowerInvariant().Contains(query) == true ||
                issue.Assignees.Any(a => a.Login.ToLowerInvariant().Contains(query))
            );
        }

        // Apply state filter
        filtered = activeFilter.State switch
        {
            IssueState.Open => filtered.Where(i => i.IsOpen),
            IssueState.Closed => filtered.Where(i => i.IsClosed),
            IssueState.All => filtered,
            _ => filtered.Where(i => i.IsOpen)
        };

        // Apply assignee filter
        if (activeFilter.Assignees.Any())
        {
            filtered = filtered.Where(issue =>
                issue.Assignees.Any(assignee =>
                    activeFilter.Assignees.Contains(assignee.Login, StringComparer.OrdinalIgnoreCase)));
        }

        // Apply label filter
        if (activeFilter.Labels.Any())
        {
            filtered = filtered.Where(issue =>
                issue.Labels.Any(label =>
                    activeFilter.Labels.Contains(label.Name, StringComparer.OrdinalIgnoreCase)));
        }

        // Apply milestone filter
        if (!string.IsNullOrWhiteSpace(activeFilter.Milestone))
        {
            filtered = filtered.Where(issue =>
                issue.Milestone?.Title.Equals(activeFilter.Milestone, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Apply author filter
        if (!string.IsNullOrWhiteSpace(activeFilter.Author))
        {
            filtered = filtered.Where(issue =>
                issue.User?.Login.Equals(activeFilter.Author, StringComparison.OrdinalIgnoreCase) == true);
        }

        // Apply date filters
        if (activeFilter.CreatedAfter.HasValue)
        {
            filtered = filtered.Where(issue => issue.CreatedAt >= activeFilter.CreatedAfter.Value);
        }

        if (activeFilter.CreatedBefore.HasValue)
        {
            filtered = filtered.Where(issue => issue.CreatedAt <= activeFilter.CreatedBefore.Value);
        }

        if (activeFilter.UpdatedAfter.HasValue)
        {
            filtered = filtered.Where(issue => issue.UpdatedAt >= activeFilter.UpdatedAfter.Value);
        }

        if (activeFilter.UpdatedBefore.HasValue)
        {
            filtered = filtered.Where(issue => issue.UpdatedAt <= activeFilter.UpdatedBefore.Value);
        }

        if (activeFilter.ClosedAfter.HasValue)
        {
            filtered = filtered.Where(issue => issue.ClosedAt >= activeFilter.ClosedAfter.Value);
        }

        if (activeFilter.ClosedBefore.HasValue)
        {
            filtered = filtered.Where(issue => issue.ClosedAt <= activeFilter.ClosedBefore.Value);
        }

        // Apply sorting
        filtered = activeFilter.SortBy switch
        {
            IssueSortBy.Created => activeFilter.SortDirection == AppSortDirection.Ascending
                ? filtered.OrderBy(i => i.CreatedAt)
                : filtered.OrderByDescending(i => i.CreatedAt),
            IssueSortBy.Updated => activeFilter.SortDirection == AppSortDirection.Ascending
                ? filtered.OrderBy(i => i.UpdatedAt)
                : filtered.OrderByDescending(i => i.UpdatedAt),
            IssueSortBy.Comments => activeFilter.SortDirection == AppSortDirection.Ascending
                ? filtered.OrderBy(i => i.CommentCount)
                : filtered.OrderByDescending(i => i.CommentCount),
            IssueSortBy.Title => activeFilter.SortDirection == AppSortDirection.Ascending
                ? filtered.OrderBy(i => i.Title)
                : filtered.OrderByDescending(i => i.Title),
            _ => filtered.OrderByDescending(i => i.UpdatedAt)
        };

        return filtered;
    }

    /// <summary>
    /// Get all saved filters
    /// </summary>
    public IEnumerable<SavedFilter> GetSavedFilters()
    {
        return _savedFilters.OrderByDescending(f => f.LastUsed);
    }

    /// <summary>
    /// Save a filter with a given name
    /// </summary>
    public async Task<SavedFilter> SaveFilterAsync(string name, IssueFilter? filter = null)
    {
        var filterToSave = filter ?? CurrentFilter;
        var savedFilter = new SavedFilter
        {
            Name = name,
            Filter = filterToSave.Clone(),
            CreatedAt = DateTime.UtcNow,
            LastUsed = DateTime.UtcNow
        };

        // Remove existing filter with same name
        _savedFilters.RemoveAll(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        _savedFilters.Add(savedFilter);

        await SaveFiltersToFileAsync();
        return savedFilter;
    }

    /// <summary>
    /// Load a saved filter by ID
    /// </summary>
    public void LoadSavedFilter(string filterId)
    {
        var savedFilter = _savedFilters.FirstOrDefault(f => f.Id == filterId);
        if (savedFilter != null)
        {
            savedFilter.LastUsed = DateTime.UtcNow;
            UpdateFilter(savedFilter.Filter);
            Task.Run(() => SaveFiltersToFileAsync()); // Execute in background with proper handling
        }
    }

    /// <summary>
    /// Delete a saved filter
    /// </summary>
    public async Task DeleteSavedFilterAsync(string filterId)
    {
        _savedFilters.RemoveAll(f => f.Id == filterId);
        await SaveFiltersToFileAsync();
    }

    /// <summary>
    /// Get filter statistics for display
    /// </summary>
    public FilterStats GetFilterStats(IEnumerable<GitHubIssue> allIssues, IEnumerable<GitHubIssue> filteredIssues)
    {
        var allCount = allIssues.Count();
        var filteredCount = filteredIssues.Count();
        
        return new FilterStats
        {
            TotalIssues = allCount,
            FilteredIssues = filteredCount,
            OpenIssues = filteredIssues.Count(i => i.IsOpen),
            ClosedIssues = filteredIssues.Count(i => i.IsClosed),
            FilteredPercentage = allCount > 0 ? (double)filteredCount / allCount * 100 : 0
        };
    }

    private void LoadSavedFilters()
    {
        try
        {
            if (File.Exists(_savedFiltersPath))
            {
                var json = File.ReadAllText(_savedFiltersPath);
                _savedFilters = JsonSerializer.Deserialize<List<SavedFilter>>(json) ?? new List<SavedFilter>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading saved filters");
            _savedFilters = new List<SavedFilter>();
        }
    }

    private async Task SaveFiltersToFileAsync()
    {
        try
        {
            var directory = Path.GetDirectoryName(_savedFiltersPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(_savedFilters, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_savedFiltersPath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving filters to file");
        }
    }
}

/// <summary>
/// Statistics about filtered results
/// </summary>
public class FilterStats
{
    public int TotalIssues { get; set; }
    public int FilteredIssues { get; set; }
    public int OpenIssues { get; set; }
    public int ClosedIssues { get; set; }
    public double FilteredPercentage { get; set; }
}