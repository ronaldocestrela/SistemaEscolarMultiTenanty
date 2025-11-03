# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-11-03

### Added
- **Custom Exception System**: Comprehensive exception handling with proper HTTP status codes
  - `ConflictException` - For resource conflicts (HTTP 409)
  - `ForbiddenException` - For access control violations (HTTP 403)
  - `IdentityException` - For authentication and identity errors (HTTP 500)
  - `NotFoundException` - For missing resources (HTTP 404)
  - `UnauthorizedException` - For unauthorized access (HTTP 401)
- **Response Wrapper System**: Standardized API response format
  - `IResponseWrapper` - Base interface for consistent responses
  - `ResponseWrapper` - Implementation for non-generic responses
  - `ResponseWrapper<T>` - Generic implementation for typed data responses
  - Support for success/failure states with structured messages
  - Async factory methods for asynchronous operations

### Removed
- Removed placeholder `Class1.cs` from Application layer

### Security
- Enhanced error handling with proper HTTP status codes
- Structured exception messages that don't expose sensitive information
- Consistent error response format across the API

### Technical Details
- All exceptions inherit from `System.Exception` and include HTTP status codes
- Response wrappers provide both synchronous and asynchronous factory methods
- Error messages are collected in lists for detailed feedback
- Type-safe response handling with generic wrapper support

## [1.0.0] - 2025-11-01

### Added
- Initial project setup with Clean Architecture
- Multi-tenant support using Finbuckle.MultiTenant 9.4.0
- ASP.NET Core Identity integration for user management
- Entity Framework Core 9.0.9 with SQL Server support
- Database migrations and seeding capabilities
- Docker Compose configuration with SQL Server container
- OpenAPI/Swagger documentation
- School entity in Domain layer
- Application and Infrastructure layers following Clean Architecture principles
- RESTful Web API with .NET 9.0
- Comprehensive README documentation (English and Portuguese)
- GitIgnore configuration for .NET projects
- Multi-tenant database contexts (TenantDbContext and ApplicationDbContext)
- Role-based authorization system with configurable password policies
- Constants for claims, permissions, and roles
- Database initialization and seeding infrastructure
- Tenant isolation with header and claims strategies

### Technical Details
- **Framework**: .NET 9.0
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, WebAPI layers)
- **Multi-tenancy**: Finbuckle.MultiTenant with EF Core store
- **Authentication**: ASP.NET Core Identity
- **Documentation**: OpenAPI/Swagger
- **Containerization**: Docker & Docker Compose

### Project Structure
```
ABCSchool/
├── src/
│   ├── core/
│   │   ├── Domain/          # Business entities and domain logic
│   │   └── Application/     # Use cases and application services
│   ├── Infrastructure/      # Data access and external services
│   └── WebAPI/             # API controllers and configuration
├── docker-compose.yaml     # Docker orchestration
├── README.md              # Comprehensive documentation
├── .gitignore            # Git ignore rules
└── ABCSchool.sln        # Visual Studio solution
```

### Database Schema
- **Tenant Management**: Multi-tenant configuration and isolation
- **Identity System**: Users, roles, and claims management
- **School Entities**: Basic school domain model

### Security Features
- Password requirements (8+ chars, mixed case, numbers, symbols)
- Unique email validation
- Role-based access control
- Tenant data isolation

[1.1.0]: https://github.com/ronaldocestrela/SistemaEscolarMultiTenanty/releases/tag/v1.1.0
[1.0.0]: https://github.com/ronaldocestrela/SistemaEscolarMultiTenanty/releases/tag/v1.0.0