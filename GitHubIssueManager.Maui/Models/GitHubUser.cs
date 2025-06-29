namespace GitHubIssueManager.Maui.Models;

public class GitHubUser
{
    public long Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}