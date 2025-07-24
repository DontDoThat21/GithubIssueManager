using System.ComponentModel.DataAnnotations;

namespace GitHubIssueManager.Maui.Models;

/// <summary>
/// Filter criteria for GitHub issues
/// </summary>
public class IssueFilter
{
    /// <summary>
    /// Text search across issue title, body, and comments
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Filter by issue state (open, closed, all)
    /// </summary>
    public IssueState State { get; set; } = IssueState.All;

    /// <summary>
    /// Filter by assignees (usernames)
    /// </summary>
    public List<string> Assignees { get; set; } = new();

    /// <summary>
    /// Filter by labels
    /// </summary>
    public List<string> Labels { get; set; } = new();

    /// <summary>
    /// Filter by milestones
    /// </summary>
    public List<string> Milestones { get; set; } = new();

    /// <summary>
    /// Filter by creation date range - start date
    /// </summary>
    public DateTime? CreatedAfter { get; set; }

    /// <summary>
    /// Filter by creation date range - end date
    /// </summary>
    public DateTime? CreatedBefore { get; set; }

    /// <summary>
    /// Filter by last updated date range - start date
    /// </summary>
    public DateTime? UpdatedAfter { get; set; }

    /// <summary>
    /// Filter by last updated date range - end date
    /// </summary>
    public DateTime? UpdatedBefore { get; set; }

    /// <summary>
    /// Include pull requests in results
    /// </summary>
    public bool IncludePullRequests { get; set; } = false;

    /// <summary>
    /// Check if any filter criteria is set
    /// </summary>
    public bool HasActiveFilters => 
        !string.IsNullOrWhiteSpace(SearchText) ||
        State != IssueState.All ||
        Assignees.Any() ||
        Labels.Any() ||
        Milestones.Any() ||
        CreatedAfter.HasValue ||
        CreatedBefore.HasValue ||
        UpdatedAfter.HasValue ||
        UpdatedBefore.HasValue ||
        IncludePullRequests;

    /// <summary>
    /// Get count of active filter criteria
    /// </summary>
    public int ActiveFilterCount
    {
        get
        {
            int count = 0;
            if (!string.IsNullOrWhiteSpace(SearchText)) count++;
            if (State != IssueState.All) count++;
            if (Assignees.Any()) count++;
            if (Labels.Any()) count++;
            if (Milestones.Any()) count++;
            if (CreatedAfter.HasValue || CreatedBefore.HasValue) count++;
            if (UpdatedAfter.HasValue || UpdatedBefore.HasValue) count++;
            if (IncludePullRequests) count++;
            return count;
        }
    }

    /// <summary>
    /// Clear all filter criteria
    /// </summary>
    public void Clear()
    {
        SearchText = null;
        State = IssueState.All;
        Assignees.Clear();
        Labels.Clear();
        Milestones.Clear();
        CreatedAfter = null;
        CreatedBefore = null;
        UpdatedAfter = null;
        UpdatedBefore = null;
        IncludePullRequests = false;
    }

    /// <summary>
    /// Create a deep copy of the filter
    /// </summary>
    public IssueFilter Clone()
    {
        return new IssueFilter
        {
            SearchText = SearchText,
            State = State,
            Assignees = new List<string>(Assignees),
            Labels = new List<string>(Labels),
            Milestones = new List<string>(Milestones),
            CreatedAfter = CreatedAfter,
            CreatedBefore = CreatedBefore,
            UpdatedAfter = UpdatedAfter,
            UpdatedBefore = UpdatedBefore,
            IncludePullRequests = IncludePullRequests
        };
    }
}

/// <summary>
/// Issue state filter options
/// </summary>
public enum IssueState
{
    [Display(Name = "All Issues")]
    All,
    [Display(Name = "Open")]
    Open,
    [Display(Name = "Closed")]
    Closed
}

/// <summary>
/// Options available for filtering (populated from GitHub API)
/// </summary>
public class IssueFilterOptions
{
    /// <summary>
    /// Available assignees for filtering
    /// </summary>
    public List<GitHubUser> Assignees { get; set; } = new();

    /// <summary>
    /// Available labels for filtering
    /// </summary>
    public List<GitHubLabel> Labels { get; set; } = new();

    /// <summary>
    /// Available milestones for filtering
    /// </summary>
    public List<GitHubMilestone> Milestones { get; set; } = new();

    /// <summary>
    /// Repository information
    /// </summary>
    public GitHubRepository? Repository { get; set; }
}

/// <summary>
/// Bulk operation options for filtered issues
/// </summary>
public class BulkOperation
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BulkOperationType Type { get; set; }
}

/// <summary>
/// Types of bulk operations available
/// </summary>
public enum BulkOperationType
{
    Close,
    Reopen,
    Assign,
    Unassign,
    AddLabel,
    RemoveLabel,
    SetMilestone,
    RemoveMilestone,
    Export
}

/// <summary>
/// Export format options
/// </summary>
public enum ExportFormat
{
    Csv,
    Json
}