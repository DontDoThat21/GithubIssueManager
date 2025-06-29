# Agent Assignment Feature

## Overview
This feature adds the ability to assign agents/users to GitHub issues directly from the repository issue detail view, with built-in protection against conflicts with existing agent assignments.

## Features Added

### 1. GitHub API Integration
- **AssignIssueAsync**: Assigns or unassigns users to/from issues
- **GetAvailableAssigneesAsync**: Retrieves all users who can be assigned to issues in a repository
- **HasAgentAssignmentAsync**: Checks if specific agent accounts are already assigned to prevent conflicts

### 2. UI Enhancements
- **Assign Button**: Each issue card now has an "Assign" button for easy access
- **Assignment Modal**: Complete modal interface for managing issue assignments
- **Agent Conflict Warning**: Automatic detection and warning when agents are already assigned
- **Visual Feedback**: Shows current assignees with avatars and names

### 3. Agent Protection
The system automatically detects and warns about assignments to common agent accounts:
- `copilot`
- `swe-copilot-agent` 
- `github-copilot`
- `copilot-agent`

When an agent is already assigned, users see a warning message: *"This issue is currently assigned to an agent. Assigning additional users may interfere with automated processes."*

## Usage

1. **Access Issues**: Navigate to any repository's issues page via the breadcrumb navigation
2. **Assign Users**: Click the "Assign" button on any issue card
3. **Select Assignees**: Use checkboxes to select/deselect assignees from available repository members
4. **Review Conflicts**: Check for any agent assignment warnings before proceeding
5. **Update Assignment**: Click "Update Assignment" to save changes

## Technical Implementation

### Service Layer (GitHubService.cs)
```csharp
// Assign users to an issue
public async Task<GitHubIssue> AssignIssueAsync(string owner, string repo, int issueNumber, IEnumerable<string> assignees)

// Get available assignees for a repository
public async Task<IEnumerable<GitHubUser>> GetAvailableAssigneesAsync(string owner, string repo)

// Check for agent assignments
public async Task<bool> HasAgentAssignmentAsync(string owner, string repo, int issueNumber, IEnumerable<string> agentLogins)
```

### UI Layer (Issues.razor)
- Added assignment modal with state management
- Integrated agent conflict detection
- Enhanced issue cards with assignment buttons
- Real-time loading states and error handling

## Security & Best Practices
- Agent detection prevents accidental interference with automated workflows
- Local state management ensures responsive UI
- Proper error handling with user feedback
- Uses existing authentication system for API access

## Screenshots
![Settings Page](https://github.com/user-attachments/assets/e82130b6-f48e-4580-9dcf-d7bc633a71dc)
*Settings page showing GitHub token configuration required for the feature*

The assignment functionality will be visible once authenticated with a valid GitHub token.