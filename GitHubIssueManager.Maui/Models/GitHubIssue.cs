namespace GitHubIssueManager.Maui.Models;

public class GitHubIssue
{
    public long Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public GitHubUser User { get; set; } = new();
    public List<GitHubUser> Assignees { get; set; } = new();
    public List<GitHubLabel> Labels { get; set; } = new();
    public GitHubMilestone? Milestone { get; set; }
    public int Comments { get; set; }
}