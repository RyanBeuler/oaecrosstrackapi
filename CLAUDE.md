# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core 9 backend API for OAE CrossTrack, a cross country and track athlete management application. Uses PostgreSQL with Entity Framework Core. Pairs with a separate Angular 19 frontend.

## Build & Development Commands

```bash
dotnet build                         # Build the project
dotnet run                           # Run dev server on http://localhost:5128
dotnet test                          # Run tests
dotnet ef migrations add <name>      # Add EF Core migration
dotnet ef database update            # Apply migrations
```

## Tech Stack

- **ASP.NET Core 9** web API
- **Entity Framework Core 9** with Npgsql (PostgreSQL)
- **JWT Bearer** authentication
- **BCrypt.Net-Next** for password hashing
- **Swagger/Swashbuckle** for API documentation

## Architecture

### Key Files

- `Program.cs` - Startup, middleware, CORS, DI setup
- `Controllers/AuthController.cs` - Authentication endpoints
- `Services/AuthService.cs` - JWT generation, password validation
- `Services/JwtSettings.cs` - JWT configuration model
- `Data/ApplicationDbContext.cs` - EF Core DbContext
- `Data/DbInitializer.cs` - Database seeding
- `Models/User.cs` - User entity
- `DTOs/AuthDtos.cs` - Request/response DTOs
- `appsettings.json` - Connection strings, JWT settings

### Authentication Flow

1. Client sends credentials to `/api/auth/login`
2. `AuthService` validates password via BCrypt
3. JWT generated with user claims
4. Token returned to client
5. Subsequent requests validated via JWT Bearer middleware

### Database

PostgreSQL on localhost:5432. User table includes:
- Id, Username, PasswordHash, Email
- FirstName, LastName
- CreatedAt, LastLoginAt

## Development Notes

- API runs on `http://localhost:5128`
- Swagger UI at `/swagger` (dev only)
- CORS allows `http://localhost:4200` and `https://oaecrosstrack.com`
- Nullable reference types enabled
- Implicit usings enabled
