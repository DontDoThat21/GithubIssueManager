using GitHubIssueManager.Maui.Models;
using Octokit;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubIssueManager.Maui.Services;

public class GitHubService
{
    private readonly GitHubClient _client;
    private readonly ILogger<GitHubService> _logger;
    private string? _token;

    public GitHubService(ILogger<GitHubService> logger)
    {
        _logger = logger;
        _client = new GitHubClient(new ProductHeaderValue("GitHubIssueManager"));
    }

    public void SetAuthentication(string token)
    {
        _client.Credentials = new Credentials(token);
        _token = token;
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

    public async Task<GitHubRepository> GetRepositoryAsync(string owner, string repo)
    {
        try
        {
            var repository = await _client.Repository.Get(owner, repo);
            return MapToGitHubRepository(repository);
        }
        catch (Octokit.ApiException apiEx)
        {
            throw (apiEx);
            //   HandleApiException(apiEx, $"fetching repository {owner}/{repo}");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Network error fetching repository {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException("Unable to connect to GitHub API. Please check your internet connection.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching repository {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<IEnumerable<GitHubIssue>> GetIssuesAsync(string owner, string repo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new UnauthorizedAccessException("GitHub authentication is required. Please configure your Personal Access Token.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubIssueManager");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://api.github.com/repos/{owner}/{repo}/issues?state=all";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("GitHub API error fetching issues for {Owner}/{Repo}: {StatusCode} {Message}", owner, repo, response.StatusCode, errorContent);
                throw response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => new UnauthorizedAccessException("GitHub authentication failed. Please check your Personal Access Token and ensure it has the required 'repo' permissions."),
                    System.Net.HttpStatusCode.Forbidden => new UnauthorizedAccessException("Access forbidden. Your GitHub token may not have permission to access this repository, or you may have exceeded the API rate limit."),
                    System.Net.HttpStatusCode.NotFound => new ArgumentException($"Repository '{owner}/{repo}' not found. Please verify the repository name and that you have access to it."),
                    _ => new InvalidOperationException($"GitHub API error: {errorContent}")
                };
            }
            var json = await response.Content.ReadAsStringAsync();
            var issues = JsonSerializer.Deserialize<List<GitHubIssue>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return issues ?? new List<GitHubIssue>();
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Network error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException("Unable to connect to GitHub API. Please check your internet connection and try again. If the problem persists, GitHub services may be temporarily unavailable.");
        }
        catch (TaskCanceledException tcEx)
        {
            _logger.LogError(tcEx, "Timeout error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new TimeoutException("Request to GitHub API timed out. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException($"An unexpected error occurred while fetching issues: {ex.Message}");
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
            if (issueNumber > int.MaxValue)
            {
                throw new ArgumentException($"Issue number {issueNumber} exceeds the maximum supported value for GitHub API calls.", nameof(issueNumber));
            }
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
            if (issueNumber > int.MaxValue)
            {
                throw new ArgumentException($"Issue number {issueNumber} exceeds the maximum supported value for GitHub API calls.", nameof(issueNumber));
            }
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

    /// <summary>
    /// Assigns the Copilot agent (copilot-swe-agent) to a GitHub issue using the REST API.
    /// </summary>
    /// <param name="owner">Repository owner</param>
    /// <param name="repo">Repository name</param>
    /// <param name="issueNumber">Issue number</param>
    /// <returns>The updated GitHubIssue if successful</returns>
    public async Task<GitHubIssue> AssignIssueToCopilotAsync(string owner, string repo, long issueNumber)
    {
        if (string.IsNullOrWhiteSpace(_token))
            throw new UnauthorizedAccessException("GitHub authentication is required. Please configure your Personal Access Token.");
        if (issueNumber <= 0)
            throw new ArgumentException("Issue number must be greater than zero.", nameof(issueNumber));

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubIssueManager");
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var url = $"https://api.github.com/repos/{owner}/{repo}/issues/{issueNumber}/assignees";
        var payload = new { assignees = new[] { "copilot-swe-agent" } };
        var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("GitHub API error assigning Copilot to issue {Owner}/{Repo} #{IssueNumber}: {StatusCode} {Message}", owner, repo, issueNumber, response.StatusCode, json);
            throw response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => new UnauthorizedAccessException("GitHub authentication failed. Please check your Personal Access Token and ensure it has the required 'repo' permissions."),
                System.Net.HttpStatusCode.Forbidden => new UnauthorizedAccessException("Access forbidden. Your GitHub token may not have permission to assign Copilot, or you may have exceeded the API rate limit."),
                System.Net.HttpStatusCode.NotFound => new ArgumentException($"Repository '{owner}/{repo}' or issue #{issueNumber} not found. Please verify the repository and issue number."),
                _ => new InvalidOperationException($"GitHub API error: {json}")
            };
        }
        var issue = JsonSerializer.Deserialize<GitHubIssue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (issue == null)
            throw new InvalidOperationException("Failed to parse updated issue from GitHub API response.");
        return issue;
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
            if (string.IsNullOrWhiteSpace(_token))
                throw new UnauthorizedAccessException("GitHub authentication is required. Please configure your Personal Access Token.");
            if (issueNumber <= 0)
                throw new ArgumentException("Issue number must be greater than zero.", nameof(issueNumber));

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubIssueManager");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var url = $"https://api.github.com/repos/{owner}/{repo}/issues/{issueNumber}";
            var response = await httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GitHub API error checking agent assignment for issue {Owner}/{Repo} #{IssueNumber}: {StatusCode} {Message}", owner, repo, issueNumber, response.StatusCode, json);
                return false;
            }
            var issue = JsonSerializer.Deserialize<GitHubIssue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (issue == null || issue.Assignees == null)
                return false;
            return issue.Assignees.Any(a => agentLogins.Contains(a.Login, StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking agent assignment for issue #{IssueNumber} in {Owner}/{Repo}", issueNumber, owner, repo);
            return false;
        }
    }

    public async Task<GitHubUser> GetUserAsync()
    {
        try
        {
            var user = await _client.User.Current();
            return new GitHubUser
            {
                Id = user.Id,
                Login = user.Login,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                HtmlUrl = user.HtmlUrl ?? string.Empty,
                Type = user.Type.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user");
            throw;
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
            CommentCount = issue.Comments
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

    public async Task<IEnumerable<GitHubLabel>> GetLabelsAsync(string owner, string repo)
    {
        try
        {
            var labels = await _client.Issue.Labels.GetAllForRepository(owner, repo);
            return labels.Select(l => new GitHubLabel
            {
                Id = l.Id,
                Name = l.Name,
                Color = l.Color,
                Description = l.Description ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching labels for {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    public async Task<IEnumerable<GitHubMilestone>> GetMilestonesAsync(string owner, string repo)
    {
        try
        {
            var milestones = await _client.Issue.Milestone.GetAllForRepository(owner, repo);
            return milestones.Select(m => new GitHubMilestone
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description ?? string.Empty,
                Number = m.Number,
                State = m.State.ToString(),
                DueOn = m.DueOn?.DateTime,
                CreatedAt = m.CreatedAt.DateTime,
                UpdatedAt = m.UpdatedAt?.DateTime ?? m.CreatedAt.DateTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching milestones for {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    public async Task<IEnumerable<GitHubUser>> GetContributorsAsync(string owner, string repo)
    {
        try
        {
            var contributors = await _client.Repository.GetAllContributors(owner, repo);
            return contributors.Select(c => new GitHubUser
            {
                Id = c.Id,
                Login = c.Login,
                AvatarUrl = c.AvatarUrl ?? string.Empty,
                HtmlUrl = c.HtmlUrl ?? string.Empty,
                Type = c.Type.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching contributors for {Owner}/{Repo}", owner, repo);
            throw;
        }
    }

    /// <summary>
    /// Get issues with advanced filtering support
    /// </summary>
    public async Task<IEnumerable<GitHubIssue>> GetIssuesAsync(string owner, string repo, 
        string state = "all", string? assignee = null, string? milestone = null, 
        string? labels = null, string? since = null, int page = 1, int perPage = 100)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new UnauthorizedAccessException("GitHub authentication is required. Please configure your Personal Access Token.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GitHubIssueManager");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var queryParams = new List<string>
            {
                $"state={state}",
                $"page={page}",
                $"per_page={perPage}"
            };

            if (!string.IsNullOrWhiteSpace(assignee))
                queryParams.Add($"assignee={Uri.EscapeDataString(assignee)}");

            if (!string.IsNullOrWhiteSpace(milestone))
                queryParams.Add($"milestone={Uri.EscapeDataString(milestone)}");

            if (!string.IsNullOrWhiteSpace(labels))
                queryParams.Add($"labels={Uri.EscapeDataString(labels)}");

            if (!string.IsNullOrWhiteSpace(since))
                queryParams.Add($"since={Uri.EscapeDataString(since)}");

            var url = $"https://api.github.com/repos/{owner}/{repo}/issues?{string.Join("&", queryParams)}";
            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("GitHub API error fetching issues for {Owner}/{Repo}: {StatusCode} {Message}", owner, repo, response.StatusCode, errorContent);
                throw response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => new UnauthorizedAccessException("GitHub authentication failed. Please check your Personal Access Token and ensure it has the required 'repo' permissions."),
                    System.Net.HttpStatusCode.Forbidden => new UnauthorizedAccessException("Access forbidden. Your GitHub token may not have permission to access this repository, or you may have exceeded the API rate limit."),
                    System.Net.HttpStatusCode.NotFound => new ArgumentException($"Repository '{owner}/{repo}' not found. Please verify the repository name and that you have access to it."),
                    _ => new InvalidOperationException($"GitHub API error: {errorContent}")
                };
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var issues = JsonSerializer.Deserialize<List<GitHubIssue>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return issues ?? new List<GitHubIssue>();
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Network error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException("Unable to connect to GitHub API. Please check your internet connection and try again. If the problem persists, GitHub services may be temporarily unavailable.");
        }
        catch (TaskCanceledException tcEx)
        {
            _logger.LogError(tcEx, "Timeout error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new TimeoutException("Request to GitHub API timed out. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching issues for {Owner}/{Repo}", owner, repo);
            throw new InvalidOperationException($"An unexpected error occurred while fetching issues: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk assign issues to users
    /// </summary>
    public async Task<List<GitHubIssue>> BulkAssignIssuesAsync(string owner, string repo, 
        IEnumerable<long> issueNumbers, IEnumerable<string> assignees)
    {
        var results = new List<GitHubIssue>();
        var assigneeList = assignees.ToList();

        foreach (var issueNumber in issueNumbers)
        {
            try
            {
                var updatedIssue = await AssignIssueAsync(owner, repo, issueNumber, assigneeList);
                results.Add(updatedIssue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning issue #{IssueNumber} in bulk operation", issueNumber);
                // Continue with other issues even if one fails
            }
        }

        return results;
    }

    /// <summary>
    /// Bulk close issues
    /// </summary>
    public async Task<List<GitHubIssue>> BulkCloseIssuesAsync(string owner, string repo, IEnumerable<long> issueNumbers)
    {
        var results = new List<GitHubIssue>();

        foreach (var issueNumber in issueNumbers)
        {
            try
            {
                ValidateIssueNumber(issueNumber);
                if (issueNumber > int.MaxValue)
                {
                    throw new ArgumentException($"Issue number {issueNumber} exceeds the maximum supported value for GitHub API calls.", nameof(issueNumber));
                }

                var issueUpdate = new IssueUpdate { State = ItemState.Closed };
                var issue = await _client.Issue.Update(owner, repo, (int)issueNumber, issueUpdate);
                results.Add(MapToGitHubIssue(issue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing issue #{IssueNumber} in bulk operation", issueNumber);
                // Continue with other issues even if one fails
            }
        }

        return results;
    }

    /// <summary>
    /// Bulk reopen issues
    /// </summary>
    public async Task<List<GitHubIssue>> BulkReopenIssuesAsync(string owner, string repo, IEnumerable<long> issueNumbers)
    {
        var results = new List<GitHubIssue>();

        foreach (var issueNumber in issueNumbers)
        {
            try
            {
                ValidateIssueNumber(issueNumber);
                if (issueNumber > int.MaxValue)
                {
                    throw new ArgumentException($"Issue number {issueNumber} exceeds the maximum supported value for GitHub API calls.", nameof(issueNumber));
                }

                var issueUpdate = new IssueUpdate { State = ItemState.Open };
                var issue = await _client.Issue.Update(owner, repo, (int)issueNumber, issueUpdate);
                results.Add(MapToGitHubIssue(issue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reopening issue #{IssueNumber} in bulk operation", issueNumber);
                // Continue with other issues even if one fails
            }
        }

        return results;
    }

    private void ValidateAuthentication()
    {
        if (_client.Credentials == null || string.IsNullOrWhiteSpace(_client.Credentials.Password))
        {
            throw new UnauthorizedAccessException("GitHub authentication is required. Please configure your Personal Access Token.");
        }
    }
}