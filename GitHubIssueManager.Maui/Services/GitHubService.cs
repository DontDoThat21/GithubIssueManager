using GitHubIssueManager.Maui.Models;
using Octokit;

namespace GitHubIssueManager.Maui.Services;

public class GitHubService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(ILogger<GitHubService> logger)
    {
        _logger = logger;
        _client = new GitHubClient(new ProductHeaderValue("GitHubIssueManager"));
    }

    public void SetAuthentication(string token)
    {
        _client.Credentials = new Credentials(token);
    }

    public async Task<IEnumerable<GitHubRepository>> GetRepositoriesAsync()
    {
        try
        {
            var repositories = await _client.Repository.GetAllForCurrent();
            return repositories.Select(MapToGitHubRepository);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repositories");
            throw;
        }
    }

    public async Task<IEnumerable<GitHubRepository>> SearchRepositoriesAsync(string query)
    {
        try
        {
            var searchRequest = new SearchRepositoriesRequest(query);
            var result = await _client.Search.SearchRepo(searchRequest);
            return result.Items.Select(MapToGitHubRepository);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching repositories");
            throw;
        }
    }

    public async Task<IEnumerable<GitHubIssue>> GetIssuesAsync(string owner, string repo)
    {
        try
        {
            var issues = await _client.Issue.GetAllForRepository(owner, repo);
            return issues.Select(MapToGitHubIssue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching issues for {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    public async Task<GitHubIssue> CreateIssueAsync(string owner, string repo, string title, string body)
    {
        try
        {
            var issueRequest = new NewIssue(title) { Body = body };
            var issue = await _client.Issue.Create(owner, repo, issueRequest);
            return MapToGitHubIssue(issue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating issue in {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    public async Task<GitHubIssue> UpdateIssueAsync(string owner, string repo, long issueNumber, string title, string body)
    {
        try
        {
            ValidateIssueNumber(issueNumber);
            
            var issueUpdate = new IssueUpdate { Title = title, Body = body };
            var issue = await _client.Issue.Update(owner, repo, (int)issueNumber, issueUpdate);
            return MapToGitHubIssue(issue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating issue #{IssueNumber} in {Owner}/{Repo}", issueNumber, owner, repo);
            throw;
        }
    }

    public async Task<GitHubIssue> AssignIssueAsync(string owner, string repo, long issueNumber, IEnumerable<string> assignees)
    {
        try
        {
            ValidateIssueNumber(issueNumber);
            
            var issueUpdate = new IssueUpdate();
            issueUpdate.Assignees.Clear();
            foreach (var assignee in assignees)
            {
                issueUpdate.Assignees.Add(assignee);
            }
            
            var issue = await _client.Issue.Update(owner, repo, (int)issueNumber, issueUpdate);
            return MapToGitHubIssue(issue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning issue #{IssueNumber} in {Owner}/{Repo}", issueNumber, owner, repo);
            throw;
        }
    }

    public async Task<IEnumerable<GitHubUser>> GetAvailableAssigneesAsync(string owner, string repo)
    {
        try
        {
            var assignees = await _client.Issue.Assignee.GetAllForRepository(owner, repo);
            return assignees.Select(a => new GitHubUser
            {
                Id = a.Id,
                Login = a.Login,
                AvatarUrl = a.AvatarUrl ?? string.Empty,
                HtmlUrl = a.HtmlUrl ?? string.Empty,
                Type = a.Type.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available assignees for {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    public async Task<bool> HasAgentAssignmentAsync(string owner, string repo, long issueNumber, IEnumerable<string> agentLogins)
    {
        try
        {
            if (issueNumber > int.MaxValue)
            {
                _logger.LogWarning("Issue number {IssueNumber} is too large to check agent assignment. Maximum supported issue number is {MaxValue}.", issueNumber, int.MaxValue);
                return false;
            }
            
            var issue = await _client.Issue.Get(owner, repo, (int)issueNumber);
            
            if (issue == null) return false;
            
            return issue.Assignees.Any(a => agentLogins.Contains(a.Login, StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking agent assignment for issue #{IssueNumber} in {Owner}/{Repo}", issueNumber, owner, repo);
            return false;
        }
    }

    private static GitHubRepository MapToGitHubRepository(Repository repo)
    {
        return new GitHubRepository
        {
            Id = repo.Id,
            Name = repo.Name,
            FullName = repo.FullName,
            Description = repo.Description ?? string.Empty,
            Private = repo.Private,
            HtmlUrl = repo.HtmlUrl,
            StargazersCount = repo.StargazersCount,
            ForksCount = repo.ForksCount,
            OpenIssuesCount = repo.OpenIssuesCount,
            Language = repo.Language ?? string.Empty,
            CreatedAt = repo.CreatedAt.DateTime,
            UpdatedAt = repo.UpdatedAt.DateTime,
            Owner = new GitHubUser
            {
                Id = repo.Owner.Id,
                Login = repo.Owner.Login,
                AvatarUrl = repo.Owner.AvatarUrl ?? string.Empty,
                HtmlUrl = repo.Owner.HtmlUrl ?? string.Empty,
                Type = repo.Owner.Type.ToString()
            }
        };
    }

    private static GitHubIssue MapToGitHubIssue(Issue issue)
    {
        return new GitHubIssue
        {
            Id = issue.Id,
            Number = issue.Number,
            Title = issue.Title,
            Body = issue.Body ?? string.Empty,
            State = issue.State.ToString(),
            HtmlUrl = issue.HtmlUrl ?? string.Empty,
            CreatedAt = issue.CreatedAt.DateTime,
            UpdatedAt = issue.UpdatedAt?.DateTime ?? issue.CreatedAt.DateTime,
            ClosedAt = issue.ClosedAt?.DateTime,
            User = new GitHubUser
            {
                Id = issue.User.Id,
                Login = issue.User.Login,
                AvatarUrl = issue.User.AvatarUrl ?? string.Empty,
                HtmlUrl = issue.User.HtmlUrl ?? string.Empty,
                Type = issue.User.Type.ToString()
            },
            Assignees = issue.Assignees.Select(a => new GitHubUser
            {
                Id = a.Id,
                Login = a.Login,
                AvatarUrl = a.AvatarUrl ?? string.Empty,
                HtmlUrl = a.HtmlUrl ?? string.Empty,
                Type = a.Type.ToString()
            }).ToList(),
            Labels = issue.Labels.Select(l => new GitHubLabel
            {
                Id = l.Id,
                Name = l.Name,
                Color = l.Color,
                Description = l.Description ?? string.Empty
            }).ToList(),
            Milestone = issue.Milestone != null ? new GitHubMilestone
            {
                Id = issue.Milestone.Id,
                Title = issue.Milestone.Title,
                Description = issue.Milestone.Description ?? string.Empty,
                Number = issue.Milestone.Number,
                State = issue.Milestone.State.ToString(),
                DueOn = issue.Milestone.DueOn?.DateTime,
                CreatedAt = issue.Milestone.CreatedAt.DateTime,
                UpdatedAt = issue.Milestone.UpdatedAt?.DateTime ?? issue.Milestone.CreatedAt.DateTime
            } : null,
            Comments = issue.Comments
        };
    }

    private void ValidateIssueNumber(long issueNumber)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentException("Issue number must be greater than zero.", nameof(issueNumber));
        }

        if (issueNumber > int.MaxValue)
        {
            throw new ArgumentException($"Issue number {issueNumber} is too large. Maximum supported issue number is {int.MaxValue}.", nameof(issueNumber));
        }
    }
}