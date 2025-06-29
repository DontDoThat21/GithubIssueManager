# Microsoft Authentication Implementation

This document describes the Microsoft authentication implementation for the MCP server.

## Overview

The application now supports standardized .NET authentication patterns using:
- **JWT Bearer tokens** for API authentication  
- **Microsoft Identity Web** for future Azure AD integration
- **ASP.NET Core Authentication middleware** for standard authentication patterns

## Authentication Types

### 1. GitHub Token Authentication (Existing)
- Used for GitHub API access
- Stored locally as personal access tokens
- Managed through the Settings UI

### 2. JWT Bearer Authentication (New)
- Used for MCP server API endpoints
- Standard ASP.NET Core JWT authentication
- Configurable issuer, audience, and signing key

### 3. Microsoft Identity Web (Optional)
- Ready for Azure AD/Microsoft identity integration
- Only activated when ClientId is configured
- Uses OpenID Connect and OAuth 2.0 protocols

## API Endpoints

### Authentication Endpoints (`/api/auth`)

- **POST /api/auth/login** - Generate JWT token
- **POST /api/auth/validate** - Validate JWT token  
- **POST /api/auth/logout** - Clear JWT token (requires auth)
- **GET /api/auth/me** - Get current user info (requires auth)

### MCP Server Endpoints (`/api/mcp`)

- **GET /api/mcp/status** - Get server status (requires auth)
- **GET /api/mcp/capabilities** - Get server capabilities (requires auth)
- **POST /api/mcp/validate-access** - Validate API access (requires auth)

## Configuration

Authentication settings are configured in `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "common",
    "ClientId": "",
    "CallbackPath": "/signin-oidc"
  },
  "Authentication": {
    "Jwt": {
      "Issuer": "https://localhost:5001",
      "Audience": "mcp-server", 
      "Key": "your-secret-key-here"
    }
  }
}
```

## Usage Examples

### 1. Login and Get Token
```bash
curl -X POST http://localhost:5225/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userId": "user@example.com", "email": "user@example.com", "roles": ["user"]}'
```

### 2. Access Protected Endpoint
```bash  
curl -X GET http://localhost:5225/api/mcp/status \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Validate Token
```bash
curl -X POST http://localhost:5225/api/auth/validate \
  -H "Content-Type: application/json" \
  -d '{"token": "YOUR_JWT_TOKEN"}'
```

## Security Features

- **JWT token expiration** (24 hours by default)
- **Secure token storage** (local file system)
- **Role-based authorization** support
- **Token validation** with configurable parameters
- **Protected API endpoints** requiring valid authentication

## Architecture

### Services Updated

1. **AuthenticationService** - Extended to support JWT tokens alongside GitHub tokens
2. **McpServerService** - Integrated with authentication validation
3. **Program.cs** - Configured authentication middleware and JWT validation

### New Components

1. **AuthController** - Handles authentication API endpoints
2. **McpController** - Demonstrates authenticated MCP server endpoints
3. **JWT Configuration** - Configurable token validation parameters

## Future Enhancements

- Azure AD integration (when ClientId is configured)
- Role-based access control for different MCP operations
- Token refresh functionality
- OAuth 2.0 client credentials flow for machine-to-machine authentication