# GitHub API Integration Documentation

## Overview

The GitHub Issue Manager application integrates with the official GitHub REST API using the Octokit.NET library to provide comprehensive issue management capabilities. This document outlines the integration approach, error handling strategies, and troubleshooting guidance.

## Architecture

### Core Components

- **GitHubService** (`Services/GitHubService.cs`): Main service class that handles all GitHub API interactions
- **Octokit.NET Library**: Official .NET client library for GitHub API
- **Authentication**: Personal Access Token (PAT) based authentication
- **Error Handling**: Comprehensive error categorization and user-friendly messaging

### API Endpoints Used

The application utilizes the following GitHub REST API endpoints:

1. **Repository Management**
   - `GET /repos/{owner}/{repo}` - Get repository information
   - `GET /user/repos` - List user repositories
   - `GET /search/repositories` - Search repositories

2. **Issue Management**
   - `GET /repos/{owner}/{repo}/issues` - List repository issues
   - `POST /repos/{owner}/{repo}/issues` - Create new issues
   - `PATCH /repos/{owner}/{repo}/issues/{issue_number}` - Update issues
   - `GET /repos/{owner}/{repo}/assignees` - Get available assignees

## Authentication

### GitHub Personal Access Token Setup

1. Navigate to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Configure the following scopes:
   - `repo` - Full control of private repositories
   - `public_repo` - Access to public repositories
   - `user` - Read user profile data

### Token Configuration in Application

1. Go to the Settings page in the application
2. Enter your Personal Access Token
3. The token is stored securely and used for all API calls

## Error Handling

The application implements comprehensive error handling to provide clear, actionable feedback to users:

### Error Categories

#### 1. Authentication Errors
- **Cause**: Missing, invalid, or expired GitHub token
- **User Message**: "GitHub authentication failed. Please check your Personal Access Token and ensure it has the required 'repo' permissions."
- **Actions**: Links to Settings page and GitHub token documentation

#### 2. Network Connectivity Issues
- **Cause**: DNS blocking, proxy restrictions, or network connectivity problems
- **User Message**: "Unable to connect to GitHub API. Please check your internet connection and try again."
- **Troubleshooting Steps**:
  - Check internet connection
  - Verify api.github.com accessibility
  - Contact IT administrator if behind corporate firewall
  - Retry operation

#### 3. Repository Access Issues
- **Cause**: Repository not found or insufficient permissions
- **User Message**: "Repository 'owner/repo' not found. Please verify the repository name and that you have access to it."
- **Actions**: Link to browse repositories or retry operation

#### 4. Rate Limiting
- **Cause**: Exceeded GitHub API rate limits
- **User Message**: "Access forbidden. Your GitHub token may not have permission to access this repository, or you may have exceeded the API rate limit."
- **Resolution**: Wait for rate limit reset or upgrade to higher rate limits

## Network Troubleshooting

### Common Network Issues

#### DNS/Proxy Blocking
If you encounter "Blocked by DNS monitoring proxy" or similar network errors:

1. **Corporate Networks**: Contact your IT administrator to allow access to:
   - `api.github.com`
   - `github.com`
   - Port 443 (HTTPS)

2. **Firewall Configuration**: Ensure outbound HTTPS connections to GitHub are allowed

3. **Proxy Settings**: Configure your application to work with corporate proxy if required

#### Connectivity Testing
You can test GitHub API connectivity using:
```bash
curl -H "Authorization: token YOUR_TOKEN" https://api.github.com/user
```

## Best Practices

### API Usage
1. **Rate Limiting Awareness**: The application respects GitHub's rate limits
2. **Efficient Caching**: Repository and issue data is cached to minimize API calls
3. **Error Recovery**: Automatic retry logic for transient errors
4. **Secure Token Storage**: Tokens are stored securely and not logged

### Security
1. **Token Permissions**: Use minimal required scopes for your use case
2. **Token Rotation**: Regularly rotate Personal Access Tokens
3. **Environment Security**: Protect your development environment where tokens are stored

## Troubleshooting Guide

### Issue: "GitHub authentication is required"
**Solution**: 
1. Go to Settings page
2. Configure valid GitHub Personal Access Token
3. Ensure token has `repo` and `user` scopes

### Issue: "Unable to connect to GitHub API"
**Solution**:
1. Check internet connectivity
2. Verify `api.github.com` is accessible
3. Check for proxy/firewall restrictions
4. Try again later (GitHub services may be down)

### Issue: "Repository not found"
**Solution**:
1. Verify repository name spelling
2. Check if repository is private and you have access
3. Ensure your token has appropriate permissions

### Issue: "API rate limit exceeded"
**Solution**:
1. Wait for rate limit reset (usually 1 hour)
2. Consider using authenticated requests for higher limits
3. Implement request throttling in high-usage scenarios

## API Reference

### GitHubService Methods

#### `GetIssuesAsync(string owner, string repo)`
Retrieves all issues for a specified repository.

**Parameters**:
- `owner`: Repository owner username
- `repo`: Repository name

**Returns**: `IEnumerable<GitHubIssue>`

**Exceptions**:
- `UnauthorizedAccessException`: Authentication required or failed
- `ArgumentException`: Repository not found
- `InvalidOperationException`: Network or API errors
- `TimeoutException`: Request timeout

#### `GetRepositoryAsync(string owner, string repo)`
Retrieves repository information.

**Parameters**:
- `owner`: Repository owner username  
- `repo`: Repository name

**Returns**: `GitHubRepository`

#### `CreateIssueAsync(string owner, string repo, string title, string body)`
Creates a new issue in the specified repository.

**Parameters**:
- `owner`: Repository owner username
- `repo`: Repository name
- `title`: Issue title
- `body`: Issue description

**Returns**: `GitHubIssue`

## Support

For additional support:
- [GitHub API Documentation](https://docs.github.com/en/rest)
- [Octokit.NET Documentation](https://octokitnet.readthedocs.io/)
- [Personal Access Token Guide](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)