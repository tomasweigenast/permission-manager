# PermissionManager

PermissionManager is a modular application designed to manage permissions for employees. It provides a clean architecture backend built with ASP.NET Core and a frontend accessible via a web interface. The system supports full-text search and event-driven operations via Elasticsearch and Kafka.


## Running
To run the project, execute `docker compose up --build -d`. It will build the containers and start all the services.
Web app is accessible via `http://localhost`, and the backend swagger endpoint via `http://localhost:5001/swagger`.

## Testing
To run unit tests, execute: 
```
dotnet test backend/PermissionManager/PermissionManager.Test
```

For integration tests, execute: 
```
dotnet test backend/PermissionManager/PermissionManager.IntegrationTest
```

## Solution Structure
The ASP.NET Core backend is structured by vertical slices using folders that reflect clear architectural boundaries:

* **.API**: The ASP.NET Core entry point of the application. Responsible for:
  * Hosting and middleware setup
  * Dependency injection configuration
  * Routing and HTTP endpoints
  * Swagger/OpenAPI configuration

* **.Domain**: Contains core domain models, entities, enums, and repository interfaces. This layer is completely agnostic of infrastructure and application logic.

* **.Application**: Implements business logic and use cases. Includes:
    * MediatR command and query handlers
    * DTOs
    * AutoMapper profiles
    * FluentValidation validators

* **.Persistence**: Contains Entity Framework Core implementations, including:
    * DbContext
    * Unit of Work pattern
    * Repository implementations

* **.Infrastructure**: Handles external service integration:
    * Kafka producer
    * Elasticsearch full-text search     indexing