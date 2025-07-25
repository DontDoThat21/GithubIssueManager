using GitHubIssueManager.Maui.Models;

namespace GitHubIssueManager.Maui.Models;

/// <summary>
/// Represents filter criteria for GitHub issues
/// </summary>
public class IssueFilter
{
    /// <summary>
    /// Search query for full-text search across title and description
    /// </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Filter by issue state (open, closed, all)
    /// </summary>
    public IssueState State { get; set; } = IssueState.Open;

    /// <summary>
    /// Filter by selected assignees (login names)
    /// </summary>
    public List<string> Assignees { get; set; } = new();

    /// <summary>
    /// Filter by selected labels (label names)
    /// </summary>
    public List<string> Labels { get; set; } = new();

    /// <summary>
    /// Filter by milestone title
    /// </summary>
    public string? Milestone { get; set; }

    /// <summary>
    /// Filter by repository (owner/repo format)
    /// </summary>
    public List<string> Repositories { get; set; } = new();

    /// <summary>
    /// Filter by creation date range
    /// </summary>
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Filter by update date range
    /// </summary>
    public DateTime? UpdatedAfter { get; set; }
    public DateTime? UpdatedBefore { get; set; }

    /// <summary>
    /// Filter by closed date range
    /// </summary>
    public DateTime? ClosedAfter { get; set; }
    public DateTime? ClosedBefore { get; set; }

    /// <summary>
    /// Filter by issue author
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Sort order for results
    /// </summary>
    public IssueSortBy SortBy { get; set; } = IssueSortBy.Updated;

    /// <summary>
    /// Sort direction
    /// </summary>
    public AppSortDirection SortDirection { get; set; } = AppSortDirection.Descending;

    /// <summary>
    /// Check if any filters are active (excluding default state filter)
    /// </summary>
    public bool HasActiveFilters => 
        !string.IsNullOrWhiteSpace(SearchQuery) ||
        State != IssueState.Open ||
        Assignees.Any() ||
        Labels.Any() ||
        !string.IsNullOrWhiteSpace(Milestone) ||
        Repositories.Any() ||
        CreatedAfter.HasValue ||
        CreatedBefore.HasValue ||
        UpdatedAfter.HasValue ||
        UpdatedBefore.HasValue ||
        ClosedAfter.HasValue ||
        ClosedBefore.HasValue ||
        !string.IsNullOrWhiteSpace(Author);

    /// <summary>
    /// Reset all filters to default values
    /// </summary>
    public void Reset()
    {
        SearchQuery = string.Empty;
        State = IssueState.Open;
        Assignees.Clear();
        Labels.Clear();
        Milestone = null;
        Repositories.Clear();
        CreatedAfter = null;
        CreatedBefore = null;
        UpdatedAfter = null;
        UpdatedBefore = null;
        ClosedAfter = null;
        ClosedBefore = null;
        Author = null;
        SortBy = IssueSortBy.Updated;
        SortDirection = AppSortDirection.Descending;
    }

    /// <summary>
    /// Create a copy of the current filter
    /// </summary>
    public IssueFilter Clone()
    {
        return new IssueFilter
        {
            SearchQuery = SearchQuery,
            State = State,
            Assignees = new List<string>(Assignees),
            Labels = new List<string>(Labels),
            Milestone = Milestone,
            Repositories = new List<string>(Repositories),
            CreatedAfter = CreatedAfter,
            CreatedBefore = CreatedBefore,
            UpdatedAfter = UpdatedAfter,
            UpdatedBefore = UpdatedBefore,
            ClosedAfter = ClosedAfter,
            ClosedBefore = ClosedBefore,
            Author = Author,
            SortBy = SortBy,
            SortDirection = SortDirection
        };
    }
}

/// <summary>
/// Issue state filter options
/// </summary>
public enum IssueState
{
    Open,
    Closed,
    All
}

/// <summary>
/// Sort options for issues
/// </summary>
public enum IssueSortBy
{
    Created,
    Updated,
    Comments,
    Title
}

/// <summary>
/// Sort direction
/// </summary>
public enum AppSortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Represents a saved filter query
/// </summary>
public class SavedFilter
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public IssueFilter Filter { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}