using GitHubIssueManager.Maui.Models;
using System.Text.Json;
using System.Text;

namespace GitHubIssueManager.Maui.Services;

/// <summary>
/// Service for filtering and searching GitHub issues
/// </summary>
public class IssueFilterService
{
    private readonly ILogger<IssueFilterService> _logger;

    public IssueFilterService(ILogger<IssueFilterService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Filter a list of issues based on the provided criteria
    /// </summary>
    public IEnumerable<GitHubIssue> FilterIssues(IEnumerable<GitHubIssue> issues, IssueFilter filter)
    {
        if (filter == null || !filter.HasActiveFilters)
            return issues;

        var filteredIssues = issues.AsEnumerable();

        // Filter by state
        if (filter.State != IssueState.All)
        {
            filteredIssues = filter.State switch
            {
                IssueState.Open => filteredIssues.Where(i => i.IsOpen),
                IssueState.Closed => filteredIssues.Where(i => i.IsClosed),
                _ => filteredIssues
            };
        }

        // Filter by assignees
        if (filter.Assignees.Any())
        {
            filteredIssues = filteredIssues.Where(issue => 
                issue.Assignees.Any(assignee => filter.Assignees.Contains(assignee.Login, StringComparer.OrdinalIgnoreCase)) ||
                (filter.Assignees.Contains("unassigned", StringComparer.OrdinalIgnoreCase) && !issue.Assignees.Any()));
        }

        // Filter by labels
        if (filter.Labels.Any())
        {
            filteredIssues = filteredIssues.Where(issue =>
                issue.Labels.Any(label => filter.Labels.Contains(label.Name, StringComparer.OrdinalIgnoreCase)) ||
                (filter.Labels.Contains("unlabeled", StringComparer.OrdinalIgnoreCase) && !issue.Labels.Any()));
        }

        // Filter by milestones
        if (filter.Milestones.Any())
        {
            filteredIssues = filteredIssues.Where(issue =>
                (issue.Milestone != null && filter.Milestones.Contains(issue.Milestone.Title, StringComparer.OrdinalIgnoreCase)) ||
                (filter.Milestones.Contains("no milestone", StringComparer.OrdinalIgnoreCase) && issue.Milestone == null));
        }

        // Filter by creation date range
        if (filter.CreatedAfter.HasValue)
        {
            filteredIssues = filteredIssues.Where(issue => issue.CreatedAt >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            filteredIssues = filteredIssues.Where(issue => issue.CreatedAt <= filter.CreatedBefore.Value);
        }

        // Filter by updated date range
        if (filter.UpdatedAfter.HasValue)
        {
            filteredIssues = filteredIssues.Where(issue => issue.UpdatedAt >= filter.UpdatedAfter.Value);
        }

        if (filter.UpdatedBefore.HasValue)
        {
            filteredIssues = filteredIssues.Where(issue => issue.UpdatedAt <= filter.UpdatedBefore.Value);
        }

        // Filter pull requests
        if (!filter.IncludePullRequests)
        {
            filteredIssues = filteredIssues.Where(issue => !issue.IsPullRequest);
        }

        // Text search (title, body, comments)
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var searchTerms = filter.SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            filteredIssues = filteredIssues.Where(issue => 
                searchTerms.All(term => 
                    (issue.Title?.Contains(term, StringComparison.OrdinalIgnoreCase) == true) ||
                    (issue.Body?.Contains(term, StringComparison.OrdinalIgnoreCase) == true) ||
                    (issue.User?.Login?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)));
        }

        return filteredIssues;
    }

    /// <summary>
    /// Search issues with advanced text matching
    /// </summary>
    public IEnumerable<GitHubIssue> SearchIssues(IEnumerable<GitHubIssue> issues, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return issues;

        var searchTerms = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return issues.Where(issue =>
        {
            // Score-based relevance matching could be added here
            var titleMatch = searchTerms.All(term => 
                issue.Title?.Contains(term, StringComparison.OrdinalIgnoreCase) == true);
            
            var bodyMatch = searchTerms.All(term => 
                issue.Body?.Contains(term, StringComparison.OrdinalIgnoreCase) == true);
            
            var authorMatch = searchTerms.Any(term => 
                issue.User?.Login?.Contains(term, StringComparison.OrdinalIgnoreCase) == true);

            var labelMatch = searchTerms.Any(term =>
                issue.Labels.Any(label => label.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));

            var numberMatch = searchTerms.Any(term =>
                term.StartsWith('#') && long.TryParse(term[1..], out var number) && issue.Number == number);

            return titleMatch || bodyMatch || authorMatch || labelMatch || numberMatch;
        });
    }

    /// <summary>
    /// Get suggested search terms based on existing issues
    /// </summary>
    public List<string> GetSearchSuggestions(IEnumerable<GitHubIssue> issues, string partialTerm)
    {
        if (string.IsNullOrWhiteSpace(partialTerm) || partialTerm.Length < 2)
            return new List<string>();

        var suggestions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Add matching labels
        foreach (var issue in issues)
        {
            foreach (var label in issue.Labels)
            {
                if (label.Name.Contains(partialTerm, StringComparison.OrdinalIgnoreCase))
                    suggestions.Add(label.Name);
            }

            // Add matching user names
            if (issue.User?.Login?.Contains(partialTerm, StringComparison.OrdinalIgnoreCase) == true)
            {
                suggestions.Add(issue.User.Login);
            }

            // Add matching assignees
            foreach (var assignee in issue.Assignees)
            {
                if (assignee.Login.Contains(partialTerm, StringComparison.OrdinalIgnoreCase))
                    suggestions.Add(assignee.Login);
            }

            // Add matching milestone titles
            if (issue.Milestone?.Title?.Contains(partialTerm, StringComparison.OrdinalIgnoreCase) == true)
            {
                suggestions.Add(issue.Milestone.Title);
            }
        }

        return suggestions.Take(10).ToList();
    }

    /// <summary>
    /// Export filtered issues to CSV format
    /// </summary>
    public string ExportToCsv(IEnumerable<GitHubIssue> issues)
    {
        var csv = new StringBuilder();
        
        // Header
        csv.AppendLine("Number,Title,State,Author,Assignees,Labels,Milestone,Created,Updated,URL");
        
        // Data rows
        foreach (var issue in issues)
        {
            var assignees = string.Join("; ", issue.Assignees.Select(a => a.Login));
            var labels = string.Join("; ", issue.Labels.Select(l => l.Name));
            var milestone = issue.Milestone?.Title ?? "";
            
            csv.AppendLine($"{EscapeCsvField(issue.Number.ToString())}," +
                          $"{EscapeCsvField(issue.Title)}," +
                          $"{EscapeCsvField(issue.State)}," +
                          $"{EscapeCsvField(issue.User?.Login ?? "")}," +
                          $"{EscapeCsvField(assignees)}," +
                          $"{EscapeCsvField(labels)}," +
                          $"{EscapeCsvField(milestone)}," +
                          $"{EscapeCsvField(issue.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                          $"{EscapeCsvField(issue.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                          $"{EscapeCsvField(issue.HtmlUrl)}");
        }
        
        return csv.ToString();
    }

    /// <summary>
    /// Export filtered issues to JSON format
    /// </summary>
    public string ExportToJson(IEnumerable<GitHubIssue> issues)
    {
        var exportData = issues.Select(issue => new
        {
            number = issue.Number,
            title = issue.Title,
            body = issue.Body,
            state = issue.State,
            author = new
            {
                login = issue.User?.Login,
                avatar_url = issue.User?.AvatarUrl
            },
            assignees = issue.Assignees.Select(a => new
            {
                login = a.Login,
                avatar_url = a.AvatarUrl
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
                due_on = issue.Milestone.DueOn
            } : null,
            created_at = issue.CreatedAt,
            updated_at = issue.UpdatedAt,
            closed_at = issue.ClosedAt,
            html_url = issue.HtmlUrl,
            comments = issue.CommentCount
        });

        return JsonSerializer.Serialize(exportData, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    /// <summary>
    /// Get available bulk operations for the current filter context
    /// </summary>
    public List<BulkOperation> GetAvailableBulkOperations()
    {
        return new List<BulkOperation>
        {
            new() { Name = "Close Issues", Icon = "Icons.Material.Filled.Close", Description = "Close selected issues", Type = BulkOperationType.Close },
            new() { Name = "Reopen Issues", Icon = "Icons.Material.Filled.Restore", Description = "Reopen selected issues", Type = BulkOperationType.Reopen },
            new() { Name = "Assign Issues", Icon = "Icons.Material.Filled.PersonAdd", Description = "Assign users to selected issues", Type = BulkOperationType.Assign },
            new() { Name = "Add Label", Icon = "Icons.Material.Filled.Label", Description = "Add label to selected issues", Type = BulkOperationType.AddLabel },
            new() { Name = "Set Milestone", Icon = "Icons.Material.Filled.Flag", Description = "Set milestone for selected issues", Type = BulkOperationType.SetMilestone },
            new() { Name = "Export", Icon = "Icons.Material.Filled.Download", Description = "Export selected issues", Type = BulkOperationType.Export }
        };
    }

    /// <summary>
    /// Sort issues by various criteria
    /// </summary>
    public IEnumerable<GitHubIssue> SortIssues(IEnumerable<GitHubIssue> issues, string sortBy, bool descending = true)
    {
        return sortBy.ToLower() switch
        {
            "created" => descending ? issues.OrderByDescending(i => i.CreatedAt) : issues.OrderBy(i => i.CreatedAt),
            "updated" => descending ? issues.OrderByDescending(i => i.UpdatedAt) : issues.OrderBy(i => i.UpdatedAt),
            "title" => descending ? issues.OrderByDescending(i => i.Title) : issues.OrderBy(i => i.Title),
            "number" => descending ? issues.OrderByDescending(i => i.Number) : issues.OrderBy(i => i.Number),
            "comments" => descending ? issues.OrderByDescending(i => i.CommentCount) : issues.OrderBy(i => i.CommentCount),
            _ => descending ? issues.OrderByDescending(i => i.UpdatedAt) : issues.OrderBy(i => i.UpdatedAt)
        };
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";

        // Escape quotes and wrap in quotes if contains comma, quote, or newline
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}