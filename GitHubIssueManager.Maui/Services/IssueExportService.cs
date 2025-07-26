using GitHubIssueManager.Maui.Models;
using GitHubIssueManager.Maui.Services;
using System.Text.Json;

namespace GitHubIssueManager.Maui.Services;

/// <summary>
/// Service for exporting filtered issues to various formats
/// </summary>
public class IssueExportService
{
    private readonly ILogger<IssueExportService> _logger;

    public IssueExportService(ILogger<IssueExportService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Export issues to CSV format
    /// </summary>
    public string ExportToCsv(IEnumerable<GitHubIssue> issues, string repositoryName)
    {
        var csv = new System.Text.StringBuilder();
        
        // Add header
        csv.AppendLine("Repository,Number,Title,State,Author,Assignees,Labels,Milestone,Created,Updated,Closed,Comments,URL");
        
        // Add data rows
        foreach (var issue in issues)
        {
            var assignees = string.Join(";", issue.Assignees.Select(a => a.Login));
            var labels = string.Join(";", issue.Labels.Select(l => l.Name));
            var milestone = issue.Milestone?.Title ?? "";
            var closedAt = issue.ClosedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
            
            csv.AppendLine($"{EscapeCsv(repositoryName)}," +
                          $"{issue.Number}," +
                          $"\"{EscapeCsv(issue.Title)}\"," +
                          $"{issue.State}," +
                          $"{issue.User?.Login ?? ""}," +
                          $"\"{EscapeCsv(assignees)}\"," +
                          $"\"{EscapeCsv(labels)}\"," +
                          $"\"{EscapeCsv(milestone)}\"," +
                          $"{issue.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                          $"{issue.UpdatedAt:yyyy-MM-dd HH:mm:ss}," +
                          $"{closedAt}," +
                          $"{issue.CommentCount}," +
                          $"{issue.HtmlUrl}");
        }
        
        return csv.ToString();
    }

    /// <summary>
    /// Export issues to JSON format
    /// </summary>
    public string ExportToJson(IEnumerable<GitHubIssue> issues, string repositoryName)
    {
        var exportData = new
        {
            metadata = new
            {
                exportedAt = DateTime.UtcNow,
                repository = repositoryName,
                totalIssues = issues.Count(),
                exportedBy = "GitHubIssueManager",
                version = "2.0"
            },
            issues = issues.Select(issue => new
            {
                repository = repositoryName,
                number = issue.Number,
                title = issue.Title,
                body = issue.Body,
                state = issue.State,
                author = new
                {
                    login = issue.User?.Login,
                    avatarUrl = issue.User?.AvatarUrl,
                    htmlUrl = issue.User?.HtmlUrl
                },
                assignees = issue.Assignees.Select(a => new
                {
                    login = a.Login,
                    avatarUrl = a.AvatarUrl,
                    htmlUrl = a.HtmlUrl
                }),
                labels = issue.Labels.Select(l => new
                {
                    name = l.Name,
                    color = l.Color,
                    description = l.Description
                }),
                milestone = issue.Milestone != null ? new
                {
                    title = issue.Milestone.Title,
                    description = issue.Milestone.Description,
                    state = issue.Milestone.State,
                    dueOn = issue.Milestone.DueOn
                } : null,
                dates = new
                {
                    createdAt = issue.CreatedAt,
                    updatedAt = issue.UpdatedAt,
                    closedAt = issue.ClosedAt
                },
                metrics = new
                {
                    commentCount = issue.CommentCount,
                    isPullRequest = issue.IsPullRequest
                },
                links = new
                {
                    htmlUrl = issue.HtmlUrl
                }
            })
        };
        
        return JsonSerializer.Serialize(exportData, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Generate export filename based on current filters and timestamp
    /// </summary>
    public string GenerateFilename(string repositoryName, IssueFilter filter, string format)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmm");
        var repoName = repositoryName.Replace("/", "-");
        
        var filterSuffix = "";
        if (filter.HasActiveFilters)
        {
            var parts = new List<string>();
            
            if (filter.State != IssueState.Open)
                parts.Add(filter.State.ToString().ToLower());
            
            if (filter.Assignees.Any())
                parts.Add($"assignees-{filter.Assignees.Count}");
            
            if (filter.Labels.Any())
                parts.Add($"labels-{filter.Labels.Count}");
            
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
                parts.Add("search");
            
            if (parts.Any())
                filterSuffix = $"-{string.Join("-", parts)}";
        }
        
        return $"issues-{repoName}-{timestamp}{filterSuffix}.{format}";
    }

    /// <summary>
    /// Get export statistics for display
    /// </summary>
    public ExportStats GetExportStats(IEnumerable<GitHubIssue> issues)
    {
        var issueList = issues.ToList();
        return new ExportStats
        {
            TotalIssues = issueList.Count,
            OpenIssues = issueList.Count(i => i.IsOpen),
            ClosedIssues = issueList.Count(i => i.IsClosed),
            UniqueAssignees = issueList.SelectMany(i => i.Assignees).Select(a => a.Login).Distinct().Count(),
            UniqueLabels = issueList.SelectMany(i => i.Labels).Select(l => l.Name).Distinct().Count(),
            DateRange = issueList.Any() ? new DateRange
            {
                StartDate = issueList.Min(i => i.CreatedAt),
                EndDate = issueList.Max(i => i.UpdatedAt)
            } : null
        };
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Replace("\"", "\"\"");
    }
}

/// <summary>
/// Statistics about the export data
/// </summary>
public class ExportStats
{
    public int TotalIssues { get; set; }
    public int OpenIssues { get; set; }
    public int ClosedIssues { get; set; }
    public int UniqueAssignees { get; set; }
    public int UniqueLabels { get; set; }
    public DateRange? DateRange { get; set; }
}

public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}