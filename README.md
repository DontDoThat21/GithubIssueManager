# GitHub Issue Manager - .NET MAUI with Blazor Hybrid

![GitHub Issue Manager](https://github.com/user-attachments/assets/7ca46940-62d5-4830-881f-393071ef62d8)

Create a cross-platform GitHub issue management application using .NET 8 with Blazor Server architecture. The application serves as both a standalone GitHub issue manager and provides a foundation for MCP (Model Context Protocol) server implementation.

## 🚀 **Project Status: Phase 1 Complete!**

### ✅ **Currently Implemented**
- **Complete Blazor Server Application** with responsive UI
- **GitHub API Integration** using Octokit.NET
- **Repository Management** - browse, search, and manage watchlists
- **Issue Management** - view and create issues
- **Authentication System** - secure GitHub token management
- **Modern UI** - Bootstrap-based responsive design
- **Local Data Persistence** - JSON-based storage

### 📱 **Live Application**
The application is fully functional and includes:
- **Dashboard**: Overview of watched repositories and statistics
- **Repository Browser**: Search and manage GitHub repositories
- **Issue Viewer**: Browse and create issues for selected repositories  
- **Settings**: Configure GitHub Personal Access Token

## 🎯 Project Goals

✅ Build a modern, cross-platform GitHub issue management tool  
✅ Implement GitHub API integration for full repository access  
✅ Provide intuitive UI for repository and issue management  
✅ Enable full CRUD operations on GitHub issues  
✅ Support multiple repositories and organizations  
🚧 Implement MCP server capabilities for AI assistant integration (Phase 3)  

## 🏗️ Technical Architecture

### Core Technologies
- **Framework**: .NET 8 with Blazor Server
- **UI Framework**: Bootstrap 5 with responsive design  
- **Backend**: GitHub REST API integration via Octokit
- **Authentication**: GitHub Personal Access Tokens
- **Data Storage**: Local JSON persistence
- **Future**: MCP Integration planned for Phase 3

### Current Project Structure
```
GitHubIssueManager.Maui/
├── Components/              # Blazor components
│   ├── Pages/              # Main application pages
│   ├── Shared/             # Shared UI components
│   └── Layout/             # Layout components
├── Services/               # Business logic services
│   ├── GitHubService.cs    # GitHub API integration ✅
│   ├── AuthenticationService.cs # Token management ✅
│   ├── RepositoryService.cs # Repository management ✅
│   └── McpServerService.cs # MCP server (Phase 3)
├── Models/                 # Data models ✅
├── wwwroot/               # Static web assets
└── README.md              # Comprehensive documentation
```

## ✨ Core Features

### 1. Repository Management ✅
- ✅ Browse and search GitHub repositories
- ✅ Add/remove repositories from watchlist  
- ✅ Display repository metadata (stars, forks, issues count)
- ✅ Support for both public and private repositories

### 2. Issue Management ✅
- ✅ View Issues: List all open issues for selected repositories
- ✅ Create Issues: Add new issues with title, description
- ✅ Issue Details: Full issue view with labels, assignees, milestones
- 🚧 Update Issues: Edit existing issue details (Phase 2)
- 🚧 Delete Issues: Remove issues with confirmation (Phase 2)
- 🚧 Advanced filtering and search (Phase 2)

### 3. User Interface ✅
- ✅ Dashboard: Overview of watched repositories and recent activity
- ✅ Repository Browser: Navigate and select repositories
- ✅ Issue List View: Clean display with metadata
- ✅ Settings Page: GitHub authentication and app configuration
- ✅ Responsive Design: Works on desktop and mobile devices

### 4. MCP Server Integration 🚧
- 🚧 Implement MCP server protocol (Phase 3)
- 🚧 REST API endpoints for external access
- 🚧 AI assistant integration capabilities
- 🚧 Real-time synchronization

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- GitHub Personal Access Token

### Running the Application

```bash
# Clone the repository
git clone https://github.com/DontDoThat21/MauiMCP.git
cd MauiMCP

# Build and run
dotnet build
cd GitHubIssueManager.Maui
dotnet run

# Open browser to http://localhost:5000
```

### Setup Instructions
1. Navigate to Settings page
2. Create a GitHub Personal Access Token with `repo`, `public_repo`, and `user` scopes
3. Enter the token in the application
4. Start browsing repositories and managing issues!

## 📋 Development Phases

### Phase 1: Foundation ✅ **COMPLETED**
- ✅ Set up Blazor project structure
- ✅ Implement basic GitHub API authentication
- ✅ Create core data models and services
- ✅ Build repository listing functionality
- ✅ Create issue viewing and basic creation
- ✅ Implement responsive UI with Bootstrap

### Phase 2: Core Issue Management 🚧 **NEXT**
- 🚧 Implement advanced issue CRUD operations
- 🚧 Build enhanced UI components
- 🚧 Add comprehensive filtering and search
- 🚧 Implement local data caching improvements

### Phase 3: MCP Server Integration 🚧 **PLANNED**
- 🚧 Port MCP server functionality
- 🚧 Integrate MCP server with application
- 🚧 Implement bidirectional synchronization
- 🚧 Add comprehensive error handling

### Phase 4: Polish & Enhancement 🚧 **FUTURE**
- 🚧 MAUI desktop applications
- 🚧 Performance optimization
- 🚧 Cross-platform testing and validation
- 🚧 Advanced GitHub integrations

## 🔧 Technical Implementation

### GitHub API Integration
- ✅ Authenticate using GitHub Personal Access Tokens
- ✅ Comprehensive error handling and logging
- ✅ Rate limiting awareness
- ✅ Support for private repositories

### Architecture Benefits
- **Service-Oriented**: Clean separation of concerns
- **Dependency Injection**: Testable and maintainable code
- **Responsive Design**: Works across all device sizes
- **Local Storage**: Secure token and data persistence
- **Extensible**: Ready for MCP server integration

## 🧪 Testing & Validation

### ✅ Verified Features
- Application builds and runs successfully
- All navigation and routing works correctly
- GitHub authentication flow functional
- Repository browsing and searching operational
- Issue creation and viewing confirmed
- Responsive UI tested across screen sizes
- Local data persistence working

## 📚 Dependencies

- **Octokit** (9.1.2): GitHub API client
- **Microsoft.Extensions.Hosting** (8.0.0): Service framework
- **System.Text.Json** (8.0.5): JSON serialization
- **Bootstrap 5**: UI framework and components

## 💡 Future Enhancements

- GitHub Actions integration
- Pull request management
- Notification system
- Bulk operations on issues
- Export/import functionality
- GitHub Copilot integration
- Real-time collaboration features

---

## 📖 Documentation

Complete documentation is available in the [application README](GitHubIssueManager.Maui/README.md) including:
- Detailed setup instructions
- Architecture overview
- API documentation
- Development guidelines
- Troubleshooting guide

**Status**: Phase 1 successfully completed with full GitHub issue management functionality! 🎉
