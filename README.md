Create a cross-platform GitHub issue management application using .NET 9 MAUI with Blazor Hybrid architecture. The application will serve as both a standalone GitHub issue manager and an MCP (Model Context Protocol) server, implementing features from the existing mcpApp console application.
🎯 Project Goals

Build a modern, cross-platform GitHub issue management tool
Implement MCP server capabilities for AI assistant integration
Provide intuitive UI for repository and issue management
Enable full CRUD operations on GitHub issues
Support multiple repositories and organizations

🏗️ Technical Architecture
Core Technologies

Framework: .NET 9 MAUI with Blazor Hybrid
UI Framework: Blazor Server/WebAssembly shared components
Backend: GitHub REST API integration
MCP Integration: Model Context Protocol server implementation
Authentication: GitHub OAuth/Personal Access Tokens

Project Structure
GitHubIssueManager.Maui/
├── Platforms/              # Platform-specific code
├── Components/              # Blazor components
│   ├── Pages/              # Main application pages
│   ├── Shared/             # Shared UI components
│   └── Layout/             # Layout components
├── Services/               # Business logic services
│   ├── GitHubService.cs    # GitHub API integration
│   ├── McpServerService.cs # MCP server implementation
│   └── RepositoryService.cs # Repository management
├── Models/                 # Data models
├── Utilities/              # Helper utilities
└── wwwroot/               # Static web assets
✨ Core Features
1. Repository Management

 Browse and search GitHub repositories
 Add/remove repositories from watchlist
 Display repository metadata (stars, forks, issues count)
 Support for both public and private repositories

2. Issue Management

 View Issues: List all open issues for selected repositories
 Create Issues: Add new issues with title, description, labels, assignees
 Update Issues: Edit existing issue details

 Modify title and description
 Add/remove labels
 Change assignees
 Update milestones


 Delete Issues: Remove issues (with confirmation)
 Filter & Search: Advanced filtering by labels, assignees, status
 Issue Details: Full issue view with comments and activity

3. MCP Server Integration

 Implement MCP server protocol from mcpApp
 Expose GitHub operations via MCP endpoints
 Support AI assistant integration
 Enable programmatic issue management
 Real-time synchronization between UI and MCP operations

4. User Interface

 Dashboard: Overview of watched repositories and recent issues
 Repository Browser: Navigate and select repositories
 Issue List View: Tabular view with sorting and filtering
 Issue Detail View: Comprehensive issue editing interface
 Settings Page: GitHub authentication and app configuration
 Dark/Light Theme: User preference theme switching

🔧 Technical Requirements
GitHub API Integration

 Authenticate using GitHub Personal Access Tokens
 Implement rate limiting and error handling
 Support GitHub Enterprise Server instances
 Cache repository and issue data for offline viewing

MCP Server Implementation

 Port existing mcpApp functionality to MAUI context
 Implement WebSocket/HTTP server for MCP communication
 Ensure thread-safe operations between UI and MCP server
 Maintain compatibility with existing MCP clients

Cross-Platform Considerations

 Windows (WinUI 3)
 macOS (Mac Catalyst)
 iOS support (future consideration)
 Android support (future consideration)

📋 Development Phases
Phase 1: Foundation (Week 1-2)

 Set up MAUI Blazor project structure
 Implement basic GitHub API authentication
 Create core data models and services
 Build basic repository listing functionality

Phase 2: Core Issue Management (Week 3-4)

 Implement issue CRUD operations
 Build main UI components
 Add filtering and search capabilities
 Implement local data caching

Phase 3: MCP Server Integration (Week 5-6)

 Port mcpApp MCP server functionality
 Integrate MCP server with MAUI application
 Implement bidirectional synchronization
 Add comprehensive error handling

Phase 4: Polish & Enhancement (Week 7-8)

 UI/UX improvements and testing
 Performance optimization
 Documentation and deployment preparation
 Cross-platform testing and validation

🔐 Security Considerations

 Secure storage of GitHub tokens
 Input validation and sanitization
 Rate limiting compliance
 Secure MCP server endpoints

📚 Dependencies

Microsoft.AspNetCore.Components.WebView.Maui
Octokit (GitHub API client)
Microsoft.Extensions.Hosting
System.Text.Json
Additional MCP protocol libraries (to be determined)

🧪 Testing Strategy

 Unit tests for services and business logic
 Integration tests for GitHub API operations
 UI testing with Blazor testing framework
 MCP server endpoint testing

📖 Documentation Requirements

 README with setup instructions
 API documentation for MCP endpoints
 User guide for issue management features
 Developer documentation for extending functionality

🚀 Success Criteria

 Successfully authenticate with GitHub
 Perform all CRUD operations on issues
 Function as a working MCP server
 Maintain feature parity with mcpApp console application
 Responsive and intuitive user interface
 Cross-platform compatibility

💡 Future Enhancements

 GitHub Actions integration
 Pull request management
 Notification system
 Bulk operations on issues
 Export/import functionality
 GitHub Copilot integration
