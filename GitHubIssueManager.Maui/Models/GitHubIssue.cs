using System.Text.Json.Serialization;
using GitHubIssueManager.Maui.Models;

/// <summary>
/// Represents a GitHub issue with proper support for large Int64 identifiers
/// </summary>
public class GitHubIssue
{
    /// <summary>
    /// Unique identifier for the issue (supports values > Int32.MaxValue)
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Issue number within the repository
    /// </summary>
    [JsonPropertyName("number")]
    public long Number { get; set; }

    /// <summary>
    /// Issue title
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Issue body/description (can be null for issues without descriptions)
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// Current state of the issue ("open" or "closed")
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = "open";

    /// <summary>
    /// URL to view the issue on GitHub
    /// </summary>
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    /// <summary>
    /// When the issue was created
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the issue was last updated
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// When the issue was closed (null if still open)
    /// </summary>
    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// User who created the issue
    /// </summary>
    [JsonPropertyName("user")]
    public GitHubUser? User { get; set; }

    /// <summary>
    /// Labels assigned to the issue
    /// </summary>
    [JsonPropertyName("labels")]
    public List<GitHubLabel> Labels { get; set; } = new();

    /// <summary>
    /// Users assigned to work on the issue
    /// </summary>
    [JsonPropertyName("assignees")]
    public List<GitHubUser> Assignees { get; set; } = new();

    /// <summary>
    /// Pull request information (null if this is a regular issue, not a PR)
    /// </summary>
    [JsonPropertyName("pull_request")]
    public object? PullRequest { get; set; }

    /// <summary>
    /// Milestone the issue is associated with
    /// </summary>
    [JsonPropertyName("milestone")]
    public GitHubMilestone? Milestone { get; set; }

    /// <summary>
    /// Number of comments on the issue
    /// </summary>
    [JsonPropertyName("comments")]
    public int CommentCount { get; set; }

    // Computed properties for easy access
    public bool IsOpen => string.Equals(State, "open", StringComparison.OrdinalIgnoreCase);
    public bool IsClosed => string.Equals(State, "closed", StringComparison.OrdinalIgnoreCase);
    public bool IsPullRequest => PullRequest != null;
}

/// <summary>
/// Represents a GitHub milestone with Int64 ID support
/// </summary>
public class GitHubMilestone
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("number")]
    public long Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("due_on")]
    public DateTime? DueOn { get; set; }

    [JsonPropertyName("closed_at")]
    public DateTime? ClosedAt { get; set; }
}