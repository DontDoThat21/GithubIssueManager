# GitHub Issue Manager - .NET MAUI with Blazor Hybrid

![GitHub Issue Manager](https://github.com/user-attachments/assets/7ca46940-62d5-4830-881f-393071ef62d8)

Create a cross-platform GitHub issue management application using .NET 8 with Blazor Server architecture. The application serves as both a standalone GitHub issue manager and provides a foundation for MCP (Model Context Protocol) server implementation.

## 🚀 **Project Status: Phase 1 Complete!**

### ✅ **Currently Implemented**
- **Complete Blazor Server Application** with responsive UI
- **GitHub API Integration** using Octokit.NET with full authentication
- **Repository Management** - browse, search, and manage watchlists
- **Advanced Issue Management** - view, create, and assign issues with conflict detection
- **🆕 Agent Assignment Feature** - Assign users to issues with automated agent conflict protection
- **🆕 Local Authentication** - JWT-based authentication system requiring no external subscriptions - consider replacing in your implementations
- **🆕 MCP Server API Endpoints** - REST API foundation for AI assistant integration
- **Authentication System** - secure GitHub token management with local JWT tokens
- **Modern UI** - Bootstrap-based responsive design with enhanced issue management
- **Local Data Persistence** - JSON-based storage with secure token handling

### 📱 **Live Application**
The application is fully functional and includes:
- **Dashboard**: Overview of watched repositories and statistics
- **Repository Browser**: Search and manage GitHub repositories
- **Issue Viewer**: Browse, create, and assign issues for selected repositories
- **🆕 Agent Assignment**: Assign users to issues with automated agent conflict detection  
- **🆕 MCP Server API**: REST endpoints for external AI assistant integration
- **Settings**: Configure GitHub Personal Access Token and authentication

## 🎯 Project Goals

✅ Build a modern, cross-platform GitHub issue management tool  
✅ Implement GitHub API integration for full repository access  
✅ Provide intuitive UI for repository and issue management  
✅ Enable full CRUD operations on GitHub issues  
✅ Support multiple repositories and organizations  
✅ Add advanced issue assignment with agent conflict detection
✅ Implement free local authentication system  
🚧 Complete MCP server capabilities for AI assistant integration (Phase 3)  

## 🏗️ Technical Architecture

### Core Technologies
- **Framework**: .NET 8 with Blazor Server
- **UI Framework**: Bootstrap 5 with responsive design  
- **Backend**: GitHub REST API integration via Octokit
- **Authentication**: GitHub Personal Access Tokens + JWT Bearer tokens
- **MCP Integration**: REST API endpoints with local JWT authentication
- **Data Storage**: Local JSON persistence with secure token management
- **Security**: Free local authentication requiring no external subscriptions

### Current Project Structure
```
GitHubIssueManager.Maui/
├── Components/              # Blazor components
│   ├── Pages/              # Main application pages
│   ├── Shared/             # Shared UI components
│   └── Layout/             # Layout components
├── Services/               # Business logic services
│   ├── GitHubService.cs    # GitHub API integration ✅
│   ├── AuthenticationService.cs # Token & JWT management ✅
│   ├── RepositoryService.cs # Repository management ✅
│   └── McpServerService.cs # MCP server with REST API ✅
├── Controllers/            # API controllers for MCP endpoints ✅
├── Models/                 # Data models ✅
├── wwwroot/               # Static web assets
└── README.md              # Comprehensive documentation
```

## ✨ Core Features

### 🆕 **Latest Features Highlight**

#### Agent Assignment System ✅
- **Smart Assignment**: Assign users to GitHub issues directly from the UI
- **Conflict Detection**: Automatic detection of AI agent assignments (copilot, swe-copilot-agent, etc.)
- **Protection Warnings**: Built-in alerts to prevent interference with automated workflows
- **Visual Feedback**: User avatars and names displayed for current assignees
- **Bulk Assignment**: Support for multiple assignee selection

#### FREE Local Authentication ✅  
- **Zero Cost**: Complete JWT authentication requiring NO subscriptions
- **Local Validation**: Tokens generated and validated entirely locally
- **Secure API**: Protected REST endpoints for MCP integration
- **No Dependencies**: No external services or cloud dependencies required
- **Enterprise Ready**: Role-based authorization support included

### 1. Repository Management ✅
- ✅ Browse and search GitHub repositories
- ✅ Add/remove repositories from watchlist  
- ✅ Display repository metadata (stars, forks, issues count)
- ✅ Support for both public and private repositories

### 2. Issue Management ✅
- ✅ View Issues: List all open issues for selected repositories
- ✅ Create Issues: Add new issues with title, description
- ✅ Issue Details: Full issue view with labels, assignees, milestones
- ✅ **Assign Issues**: Assign users to issues with automated agent conflict detection
- ✅ **Agent Protection**: Built-in warnings for assignments to automated agent accounts
- 🚧 Update Issues: Edit existing issue details (Phase 2)
- 🚧 Delete Issues: Remove issues with confirmation (Phase 2)
- 🚧 Advanced filtering and search (Phase 2)

### 3. User Interface ✅
- ✅ Dashboard: Overview of watched repositories and recent activity
- ✅ Repository Browser: Navigate and select repositories
- ✅ Issue List View: Clean display with metadata
- ✅ Settings Page: GitHub authentication and app configuration
- ✅ Responsive Design: Works on desktop and mobile devices

### 4. MCP Server Integration ✅
- ✅ **REST API Endpoints**: `/api/auth` and `/api/mcp` endpoints implemented
- ✅ **FREE Local Authentication**: JWT-based authentication requiring no subscriptions
- ✅ **Token Management**: Secure JWT token generation and validation
- ✅ **API Foundation**: Complete authentication infrastructure for AI integration
- 🚧 WebSocket support for real-time updates (Phase 3)
- 🚧 Full MCP protocol implementation (Phase 3)
- 🚧 AI assistant integration capabilities (Phase 3)
- 🚧 Cross-platform synchronization (Phase 3)

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

### 🆕 **Using New Features**

#### Agent Assignment
1. Navigate to any repository's issues via breadcrumb navigation
2. Click the "Assign" button on any issue card
3. Select/deselect assignees using checkboxes
4. Review any agent assignment warnings before proceeding  
5. Click "Update Assignment" to save changes

#### MCP API Access
1. Generate JWT token via `/api/auth/login` endpoint
2. Use token in Authorization header: `Bearer YOUR_JWT_TOKEN`
3. Access protected endpoints like `/api/mcp/status`
4. All authentication is free and runs locally!

## 📋 Development Phases

### Phase 1: Foundation ✅ **COMPLETED**
- ✅ Set up Blazor project structure
- ✅ Implement basic GitHub API authentication
- ✅ Create core data models and services
- ✅ Build repository listing functionality
- ✅ Create issue viewing and basic creation
- ✅ Implement responsive UI with Bootstrap
- ✅ **Add advanced issue assignment with agent conflict detection**
- ✅ **Implement free local JWT authentication system**
- ✅ **Build MCP server API foundation with secure endpoints**

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
- ✅ **Advanced Issue Assignment**: Assign/unassign users with conflict detection
- ✅ **Agent Protection**: Automatic detection of AI agent accounts

### MCP Server Architecture
- ✅ **Free Local Authentication**: JWT tokens generated and validated locally
- ✅ **API Endpoints**: Complete `/api/auth` and `/api/mcp` REST endpoints
- ✅ **Zero Dependencies**: No external services or subscriptions required
- ✅ **Security**: Secure token management with configurable expiration
- ✅ **Extensible**: Ready for full MCP protocol implementation

### Architecture Benefits
- **Service-Oriented**: Clean separation of concerns
- **Dependency Injection**: Testable and maintainable code
- **Responsive Design**: Works across all device sizes
- **Local Storage**: Secure token and data persistence
- **Extensible**: Ready for MCP server integration

## 🔌 **API Documentation**

### Authentication Endpoints (`/api/auth`) ✅
```bash
# Generate JWT Token (free)
POST /api/auth/login
Content-Type: application/json
{
  "userId": "user@example.com",
  "email": "user@example.com", 
  "roles": ["user"]
}

# Validate Token
POST /api/auth/validate
Content-Type: application/json
{
  "token": "YOUR_JWT_TOKEN"
}

# Get Current User (requires auth)
GET /api/auth/me
Authorization: Bearer YOUR_JWT_TOKEN

# Logout (requires auth)  
POST /api/auth/logout
Authorization: Bearer YOUR_JWT_TOKEN
```

### MCP Server Endpoints (`/api/mcp`) ✅
```bash
# Get Server Status (requires auth)
GET /api/mcp/status
Authorization: Bearer YOUR_JWT_TOKEN

# Get Server Capabilities (requires auth)
GET /api/mcp/capabilities  
Authorization: Bearer YOUR_JWT_TOKEN

# Validate API Access (requires auth)
POST /api/mcp/validate-access
Authorization: Bearer YOUR_JWT_TOKEN
```

### Usage Examples
```bash
# 1. Login and get token
curl -X POST http://localhost:5225/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userId": "user@example.com", "email": "user@example.com", "roles": ["user"]}'

# 2. Access protected endpoint  
curl -X GET http://localhost:5225/api/mcp/status \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**All API operations are free and require no external services!**

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

### Core Dependencies
- **Octokit** (9.1.2): GitHub API client
- **Microsoft.Extensions.Hosting** (8.0.0): Service framework
- **System.Text.Json** (8.0.5): JSON serialization
- **Bootstrap 5**: UI framework and components

### Authentication & Security
- **System.IdentityModel.Tokens.Jwt**: JWT token handling
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication middleware
- **Microsoft.AspNetCore.Authorization**: Role-based authorization

### Future Integrations
- **SignalR** (planned): Real-time communications
- **Entity Framework Core** (planned): Database integration
- **Microsoft.Maui** (planned): Cross-platform native apps

## 🗺️ **Comprehensive Future Roadmap**

### Phase 2: Advanced Issue Management 🚧 **IN PROGRESS** (Q1 2024)
**Core Issue Operations**
- 🚧 Edit Issues: Comprehensive editing of title, description, labels
- 🚧 Close/Reopen Issues: Full issue lifecycle management
- 🚧 Bulk Operations: Multi-select and batch actions on issues
- 🚧 Advanced Search: Filter by assignee, labels, milestones, status
- 🚧 Issue Templates: Predefined templates for common issue types
- 🚧 Issue Comments: View and add comments to issues

**Enhanced UI/UX**
- 🚧 Improved Issue Detail View: Rich text editing, inline attachments
- 🚧 Drag & Drop Interface: Visual assignment and status changes
- 🚧 Real-time Updates: Live synchronization with GitHub changes
- 🚧 Dark/Light Theme Toggle: Enhanced theming options
- 🚧 Mobile Optimization: Touch-friendly interface improvements

### Phase 3: Full MCP Integration 🚧 **PLANNED** (Q2 2024)
**MCP Protocol Implementation**
- 🚧 Complete MCP Server: Full protocol compliance for AI assistants
- 🚧 WebSocket Support: Real-time bidirectional communication
- 🚧 Tool Registry: Register and manage available tools and capabilities
- 🚧 Session Management: Multi-client session handling
- 🚧 Resource Discovery: Dynamic capability advertisement

**AI Assistant Integration**
- 🚧 Claude Integration: Direct integration with Anthropic's Claude
- 🚧 OpenAI Compatibility: Support for ChatGPT and other OpenAI models
- 🚧 Automated Issue Triage: AI-powered issue categorization and assignment
- 🚧 Smart Suggestions: AI-generated issue descriptions and solutions
- 🚧 Code Analysis: AI-powered code review and issue detection

### Phase 4: Enterprise & Collaboration 🚧 **PLANNED** (Q3 2024)
**Team Collaboration Features**
- 🚧 Multi-User Support: Team workspaces and role-based access
- 🚧 Notification System: Real-time alerts and email notifications
- 🚧 Activity Streams: Team activity feeds and audit logs
- 🚧 Project Boards: Kanban-style project management integration
- 🚧 Time Tracking: Built-in time tracking and reporting

**Integration & Automation**
- 🚧 GitHub Actions: Trigger workflows from issue actions
- 🚧 Webhooks: Custom webhook support for external integrations
- 🚧 API Extensions: Custom plugins and third-party integrations
- 🚧 JIRA Sync: Bi-directional synchronization with JIRA
- 🚧 Slack/Teams Integration: Direct messaging and notifications

### Phase 5: Cross-Platform & Performance 🚧 **PLANNED** (Q4 2024)
**Native Applications**
- 🚧 MAUI Desktop Apps: Native Windows, macOS, and Linux applications
- 🚧 Mobile Apps: iOS and Android native applications
- 🚧 Offline Support: Local caching and offline issue management
- 🚧 Cross-Device Sync: Seamless synchronization across devices

**Performance & Scalability**
- 🚧 Database Integration: SQL Server, PostgreSQL, or MySQL backend
- 🚧 Caching Layer: Redis-based caching for improved performance
- 🚧 Background Processing: Asynchronous task processing
- 🚧 Load Testing: Performance optimization for large teams
- 🚧 Monitoring: Application performance monitoring and analytics

### Phase 6: Advanced AI & Analytics 🚧 **FUTURE** (2025+)
**Advanced AI Features**
- 🚧 Predictive Analytics: Issue resolution time predictions
- 🚧 Automated Testing: AI-generated test cases for issues
- 🚧 Code Generation: AI-powered code suggestions and fixes
- 🚧 Natural Language Queries: Chat-based issue management
- 🚧 Smart Routing: Intelligent issue assignment based on expertise

**Enterprise Analytics**
- 🚧 Advanced Reporting: Comprehensive team and project analytics
- 🚧 Custom Dashboards: Personalized metrics and KPI tracking
- 🚧 Trend Analysis: Historical data analysis and insights
- 🚧 Compliance Reporting: Audit trails and compliance documentation
- 🚧 Performance Metrics: Developer productivity and team health metrics

## 🎯 **Short-term Priority Features** (Next 30 days)
1. **Issue Editing**: Complete CRUD operations for issues
2. **Enhanced Search**: Advanced filtering and search capabilities  
3. **Bulk Operations**: Multi-select actions for efficiency
4. **Mobile UI**: Touch-optimized interface improvements
5. **Performance**: Caching and loading optimizations

## 🔮 **Long-term Vision** (6-12 months)
- **AI-First Approach**: Every feature enhanced with AI capabilities
- **Enterprise Ready**: Full team collaboration and management features  
- **Cross-Platform**: Native apps for all major platforms
- **Ecosystem Integration**: Deep integration with development tools
- **Open Source Community**: Public API and plugin ecosystem

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

### Comprehensive Documentation Available:
- **[Application README](GitHubIssueManager.Maui/README.md)**: Detailed setup, architecture, and development guidelines
- **[Agent Assignment Feature](AGENT_ASSIGNMENT_FEATURE.md)**: Complete guide to the new issue assignment system
- **[Free Authentication Guide](FREE_AUTHENTICATION.md)**: JWT authentication setup and API usage  
- **API Documentation**: Complete REST API reference (above)
- **Troubleshooting Guide**: Issue resolution and debugging tips

### Key Documentation Highlights:
- ✅ **Setup Instructions**: Step-by-step installation and configuration
- ✅ **Architecture Overview**: Technical implementation details  
- ✅ **Feature Guides**: How to use agent assignment and authentication
- ✅ **API Reference**: Complete endpoint documentation with examples
- ✅ **Security Notes**: Authentication and token management best practices

**Status**: Phase 1 successfully completed with advanced GitHub issue management and MCP integration foundation! 🚀✨
