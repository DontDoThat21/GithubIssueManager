namespace GitHubIssueManager.Maui.Models;

public class GitHubMilestone
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Number { get; set; }
    public string State { get; set; } = string.Empty;
    public DateTime? DueOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}