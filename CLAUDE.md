# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Build and Run
```bash
# Build the entire solution
dotnet build

# Run the application from the MAUI project directory
cd GitHubIssueManager.Maui
dotnet run

# Application will be available at:
# HTTP: http://localhost:5225
# HTTPS: https://localhost:7270
```

### Testing
No specific test commands are configured in this project. Check with the user for testing requirements.

## Project Architecture

### Technology Stack
- **.NET 8** with Blazor Server (not MAUI despite folder naming)
- **MudBlazor** UI framework (v8.10.0) 
- **Octokit** for GitHub API integration (v9.1.2)
- **JWT Bearer authentication** for local API endpoints
- **Bootstrap 5** for responsive design

### Key Components

#### Services (`GitHubIssueManager.Maui/Services/`)
- `GitHubService.cs`: GitHub API integration using Octokit
- `AuthenticationService.cs`: JWT token management and GitHub token handling
- `RepositoryService.cs`: Repository watchlist and management
- `McpServerService.cs`: MCP (Model Context Protocol) server functionality

#### Controllers (`GitHubIssueManager.Maui/Controllers/`)
- `AuthController.cs`: JWT authentication endpoints at `/api/auth`
- `McpController.cs`: MCP server API endpoints at `/api/mcp`

#### Pages (`GitHubIssueManager.Maui/Components/Pages/`)
- `Home.razor`: Dashboard with repository overview
- `Repositories.razor`: Repository browsing and management
- `Issues.razor`: Issue viewing and management interface
- `Settings.razor`: GitHub token configuration

### Authentication System
The application uses a dual authentication approach:
1. **GitHub Personal Access Tokens** for GitHub API access (stored in `Data/auth.json`)
2. **Local JWT tokens** for API endpoint protection (free, no external services)

### Data Storage
- Local JSON files in `Data/` directory:
  - `auth.json`: Encrypted GitHub authentication tokens
  - `watched-repositories.json`: User's watched repository list

### API Endpoints
- `/api/auth/*`: JWT authentication (login, validate, logout)
- `/api/mcp/*`: MCP server capabilities and status

### Key Features
- **Agent Assignment Protection**: Detects AI agent accounts (copilot, swe-copilot-agent) and warns before assignment
- **Repository Management**: Add/remove repositories from watchlist
- **Issue Management**: View, create, and assign GitHub issues
- **Responsive UI**: Works across desktop and mobile devices

## Development Notes

### Service Registration
Services use dependency injection with specific lifetimes:
- `GitHubService`: Scoped (for HttpClient best practices)
- `AuthenticationService`: Singleton
- `RepositoryService`: Singleton  
- `McpServerService`: Scoped

### Configuration
- Launch settings in `Properties/launchSettings.json`
- JWT configuration expected in `appsettings.json` under `Authentication:Jwt`
- GitHub API rate limiting is handled automatically

### Security Considerations
- GitHub tokens are encrypted before storage
- JWT tokens use symmetric key validation
- All API endpoints require proper authorization headers