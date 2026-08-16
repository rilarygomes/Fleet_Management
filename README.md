# FleetManagement

## Overview

FleetManagement is a RESTful API built with .NET 9 for managing drivers, vehicles, and trips.

The project follows **Clean Architecture** principles and applies a **CQRS-style separation** between commands and queries. It includes domain rules, validation, structured logging, database migrations, seed data, unit tests, and integration tests.

## Technologies

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- Serilog
- Swagger / OpenAPI
- xUnit
- Moq
- Bogus

## Architecture

The solution follows Clean Architecture principles:

```text
API
 |
 v
Application
 |
 v
Domain
 ^
 |
Infrastructure
```

### Domain

Contains entities, business rules, exceptions, and repository contracts.

### Application

Contains use cases organized into Commands and Queries, DTOs, validators, and handlers.

### Infrastructure

Contains Entity Framework Core, repositories, database configuration, migrations, and persistence concerns.

### API

Contains controllers, Swagger configuration, middleware, dependency injection, and HTTP concerns.

### Tests

Contains unit tests and integration tests isolated from the development database.

## Project Structure

```text
src/
├── FleetManagement.Api
├── FleetManagement.Application
├── FleetManagement.Domain
└── FleetManagement.Infrastructure

tests/
├── FleetManagement.UnitTests
└── FleetManagement.IntegrationTests
```

## Business Rules

### Driver

- Driver name is required and must contain at least 3 characters.
- License number must contain exactly 11 digits.
- License number must be unique.
- License expiration date must be in the future.
- A driver with associated trips cannot be deleted.

### Vehicle

- License plate is required and must contain exactly 7 characters.
- License plate must be unique.
- Model is required and must contain at least 2 characters.
- Manufacturing year must be between 1960 and the current year.
- A vehicle with associated trips cannot be deleted.

### Trip

- Driver and vehicle must exist.
- Start date cannot be in the past.
- End date must be after or equal to the start date.
- A driver cannot have overlapping trips.
- A vehicle cannot have overlapping trips.
- A trip that has already started cannot be updated.

## Database

The application uses **SQLite** through Entity Framework Core.

Database migrations are included in the repository.

To apply the migrations:

```bash
dotnet ef database update
```

The application includes seed data with more than 100 records across drivers, vehicles, and trips.

Seed data is automatically generated when the application starts in Development or Production and the database is empty.

## Running the Application

### Requirements

- .NET 9 SDK
- Git

### Clone the Repository

```bash
git clone <repository-url>
cd fleet-management-api
```

### Restore Dependencies

```bash
dotnet restore
```

### Apply Migrations

```bash
dotnet ef database update
```

### Run the API

```bash
dotnet run --project src/FleetManagement.Api
```

Swagger is available at the address displayed by the application when running locally.

## API Endpoints

### Drivers

```text
GET    /api/driver
GET    /api/driver/{id}
POST   /api/driver
PUT    /api/driver/{id}
DELETE /api/driver/{id}
```

Driver search:

```http
GET /api/driver?name=Carlos
```

### Vehicles

```text
GET    /api/vehicle
GET    /api/vehicle/{id}
POST   /api/vehicle
PUT    /api/vehicle/{id}
DELETE /api/vehicle/{id}
```

Vehicle search:

```http
GET /api/vehicle?licensePlate=ABC
```

### Trips

```text
GET    /api/trip
GET    /api/trip/{id}
POST   /api/trip
PUT    /api/trip/{id}
DELETE /api/trip/{id}
```

Trip filters:

```http
GET /api/trip?driverId={id}
GET /api/trip?vehicleId={id}
GET /api/trip?startDate=2026-09-01&endDate=2026-09-30
```

## Validation and Error Handling

FluentValidation is used for request validation.

Validation exceptions are handled globally and returned as HTTP `400 Bad Request` responses with consistent error information.

Examples of HTTP responses used by the API:

| Status Code | Description |
|---|---|
| `200 OK` | Request completed successfully |
| `201 Created` | Resource created successfully |
| `204 No Content` | Request completed without response content |
| `400 Bad Request` | Invalid request or validation error |
| `404 Not Found` | Resource not found |
| `500 Internal Server Error` | Unexpected server error |

## Observability

Structured logging is implemented using **Serilog**.

Logs are written to:

- Console
- Rolling log files

Structured properties such as entity IDs and relevant request information are included in application logs.

## Testing

Run all tests with:

```bash
dotnet test
```

The solution includes:

- Unit tests for Commands, Queries, handlers, and validators
- Integration tests for API endpoints

Integration tests use an isolated in-memory database and do not rely on the development or production database.

## API Documentation

Swagger / OpenAPI is available in Development mode.

Request and response examples are included for the main Driver, Vehicle, and Trip operations.

## Cross-Platform Support

The solution targets **.NET 9** and uses cross-platform technologies:

- ASP.NET Core
- Entity Framework Core
- SQLite
- .NET CLI

The application does not depend on Windows-specific APIs and is designed to run on:

- Windows
- macOS
- Linux

## Design Decisions

### Clean Architecture

Clean Architecture was chosen to separate business logic from infrastructure and presentation concerns.

### CQRS-Style Organization

Commands are used for operations that change application state, while Queries are used for read operations.

This keeps each use case focused and independently testable.

### SQLite

SQLite was selected to make the challenge easy to execute locally without requiring an external database server.

### Explicit Handlers Instead of MediatR

Handlers are injected directly to keep the dependency graph explicit and avoid introducing unnecessary dependencies for the scope of this project.

## Future Improvements

Possible future improvements include:

- Authentication and authorization
- API versioning
- Docker support
- Health checks
- CI/CD pipeline
- Pagination for list endpoints
- Asynchronous repository and handler operations