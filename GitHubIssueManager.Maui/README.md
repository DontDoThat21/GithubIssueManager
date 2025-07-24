# GitHub Issue Manager

A cross-platform GitHub issue management application built with .NET 8 and Blazor Server. This application serves as both a standalone GitHub issue manager and provides a foundation for MCP (Model Context Protocol) server capabilities.

![GitHub Issue Manager Homepage](https://github.com/user-attachments/assets/7ca46940-62d5-4830-881f-393071ef62d8)

## 🎯 Features

### ✅ Currently Implemented (Phase 1)

- **GitHub Authentication**: Secure token-based authentication with local storage
- **Repository Management**: 
  - Browse and search GitHub repositories
  - Add/remove repositories from watchlist
  - View repository metadata (stars, forks, issues count)
  - Support for both public and private repositories
- **Issue Management**:
  - View all issues for selected repositories
  - Create new issues with title and description
  - Filter and display issue details including labels, assignees, and milestones
- **Modern UI**: Responsive Bootstrap-based interface with dark theme
- **Local Data Persistence**: Watchlist and authentication data stored locally
- **Toast Notifications**: Real-time feedback for user actions using Blazor Bootstrap Toasts

### 🚧 Planned Features (Future Phases)

- **Advanced Issue Management**: Edit, close, and assign issues
- **MCP Server Integration**: REST API endpoints for AI assistant integration
- **Bulk Operations**: Mass operations on issues
- **Enhanced Filtering**: Advanced search and filtering capabilities
- **Cross-Platform**: MAUI desktop applications for Windows and macOS

## 🏗️ Architecture

### Technology Stack

- **Framework**: .NET 8.0 with Blazor Server
- **GitHub Integration**: Octokit.NET library
- **UI Framework**: Bootstrap 5 with responsive design
- **Data Storage**: JSON-based local persistence
- **Authentication**: GitHub Personal Access Tokens

### Project Structure

```
GitHubIssueManager.Maui/
├── Components/              # Blazor components
│   ├── Pages/              # Main application pages
│   │   ├── Home.razor      # Dashboard and overview
│   │   ├── Repositories.razor # Repository browsing and management
│   │   ├── Issues.razor    # Issue viewing and creation
│   │   └── Settings.razor  # Authentication and configuration
│   ├── Layout/            # Layout components
│   └── Shared/            # Reusable UI components
├── Services/              # Business logic services
│   ├── GitHubService.cs   # GitHub API integration
│   ├── AuthenticationService.cs # Token management
│   ├── RepositoryService.cs # Watchlist management
│   └── McpServerService.cs # MCP server (placeholder)
├── Models/                # Data models
│   ├── GitHubRepository.cs
│   ├── GitHubIssue.cs
│   ├── GitHubUser.cs
│   ├── GitHubLabel.cs
│   └── GitHubMilestone.cs
├── Properties/            # Application properties
└── wwwroot/              # Static web assets
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- A GitHub account with Personal Access Token

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/DontDoThat21/MauiMCP.git
   cd MauiMCP
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the application:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   cd GitHubIssueManager.Maui
   dotnet run
   ```

5. Open your browser and navigate to `http://localhost:5000`

### Configuration

1. **GitHub Personal Access Token Setup**:
   - Go to [GitHub Settings → Developer settings → Personal access tokens](https://github.com/settings/tokens)
   - Click "Generate new token" → "Generate new token (classic)"
   - Give it a descriptive name like "GitHub Issue Manager"
   - Select the following scopes:
     - `repo` - Full control of private repositories
     - `public_repo` - Access public repositories  
     - `user` - Read user profile data
   - Click "Generate token" and copy the token value
   
2. **Application Configuration**:
   - Navigate to the Settings page in the application
   - Paste your GitHub token in the provided field
   - Click "Save Token"

## 📋 Usage Guide

### Managing Repositories

1. **Browse Repositories**:
   - Navigate to the "Repositories" page
   - Use "My Repos" to load your repositories
   - Use the search box to find specific repositories

2. **Add to Watchlist**:
   - Click "Add to Watchlist" on any repository card
   - Watched repositories appear on the dashboard

3. **View Issues**:
   - Click "View Issues" on any watched repository
   - Browse existing issues or create new ones

### Managing Issues

1. **View Issues**:
   - Navigate to a repository's issues page
   - See all open issues with labels, assignees, and metadata

2. **Create Issues**:
   - Click "New Issue" button
   - Fill in title and description
   - Submit to create the issue on GitHub

## 🔧 Development

### Architecture Patterns

- **Service-Oriented**: Business logic separated into services
- **Dependency Injection**: All services registered and injected
- **Repository Pattern**: Data access abstracted through services
- **Component-Based UI**: Blazor components for reusable UI elements

### Key Services

- **GitHubService**: Handles all GitHub API interactions using Octokit
- **AuthenticationService**: Manages GitHub token storage and validation  
- **RepositoryService**: Manages the user's watchlist with local persistence
- **McpServerService**: Placeholder for future MCP server implementation

### Data Models

All GitHub entities are mapped to local models for consistent data handling:
- **GitHubRepository**: Repository information and metadata
- **GitHubIssue**: Issue details, labels, assignees, milestones
- **GitHubUser**: User profile information
- **GitHubLabel**: Issue labels with colors
- **GitHubMilestone**: Project milestones

## 🧪 Testing

### Manual Testing

The application has been tested with:
- ✅ Successful build and startup
- ✅ Navigation between all pages
- ✅ Authentication flow (token input/storage)
- ✅ Repository browsing (requires valid token)
- ✅ Issue viewing and creation (requires valid token)
- ✅ Responsive UI design

### Automated Testing

Future phases will include:
- Unit tests for services and business logic
- Integration tests for GitHub API operations
- UI testing with Blazor testing framework

## 📦 Dependencies

- **Octokit** (9.1.2): GitHub API client library
- **Microsoft.Extensions.Hosting** (8.0.0): Service hosting and DI
- **System.Text.Json** (8.0.5): JSON serialization for local storage
- **Microsoft.AspNetCore.Components.WebView.Maui**: For future MAUI integration
- **Blazor Bootstrap Toasts**: Provides toast notifications for user feedback (https://demos.blazorbootstrap.com/toasts)

## 🗺️ Roadmap

### Phase 1: Foundation ✅ **COMPLETED**
- [x] Basic Blazor project structure
- [x] GitHub API authentication
- [x] Core data models and services
- [x] Repository listing and management
- [x] Basic issue viewing and creation
- [x] Settings and configuration UI

### Phase 2: Advanced Issue Management (Planned)
- [ ] Edit existing issues
- [ ] Close/reopen issues
- [ ] Manage labels and assignees
- [ ] Milestone management
- [ ] Advanced filtering and search
- [ ] Bulk operations

### Phase 3: MCP Server Integration (Planned)
- [ ] REST API endpoints
- [ ] MCP protocol implementation
- [ ] WebSocket support for real-time updates
- [ ] AI assistant integration
- [ ] Cross-platform MAUI applications

### Phase 4: Enhancement (Planned)
- [ ] GitHub Actions integration
- [ ] Pull request management  
- [ ] Notification system
- [ ] Export/import functionality
- [ ] Performance optimizations

## 🤝 Contributing

This project is part of a GitHub issue management initiative. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🛠️ Technical Notes

### Security Considerations

- GitHub tokens are stored locally using JSON serialization
- No tokens are transmitted to external services (except GitHub API)
- Input validation on all user inputs
- Secure HTTPS communication with GitHub API

### Performance Considerations

- Singleton services for efficient resource usage
- Local caching of repository and user data
- Responsive UI with Bootstrap for cross-device compatibility
- Async/await patterns for non-blocking operations

### Known Limitations

- Currently runs as a web application (MAUI integration planned)
- Limited to GitHub REST API capabilities
- No real-time updates (polling-based)
- Single-user application (no multi-tenancy)

## 📞 Support

For questions, issues, or contributions, please:
- Open an issue on GitHub
- Check the documentation in this README
- Review the code comments for implementation details

---

*Built with ♥ using .NET 8 and Blazor Server*