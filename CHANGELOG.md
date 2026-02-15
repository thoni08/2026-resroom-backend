# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.2] - 2026-02-15

### Added
- **Pagination Improvements:** Updated Pagination result to display all items when a specific criteria is triggered.

### Fixed
- **Max Capacity Filter in Rooms:** Implemented 'missing' `MaxCapacity` filter in Rooms to ensure detailed room capacity filtering.
- **Specifically Time Validation:** Updated validation error for room availability

## [1.0.1] - 2026-02-09

### Added
- **Global Error Handling:** Implemented `ExceptionMiddleware` to catch unhandled system exceptions (e.g., database failures) and return standardized JSON responses.
- **Validation Standards:** Updated all controllers to use `ValidationProblem()` details (RFC 7807) for consistent error 4XX responses.
- **JSON Handling:** Added `ReservationStatusConverter` to provide user-friendly error messages when invalid Enum values (e.g., "status": "invalid") are submitted.
- **Database Seeder:** Implemented `ModelBuilderExtensions` to automatically populate the database with initial sample data (Rooms) during migrations.

### Fixed
- **Room Sorting:** Fixed a bug in `GET /api/rooms` where sorting by `name` ignored the `sortDirection` parameter and always defaulted to Ascending (A-Z).

## [1.0.0] - 2026-02-08

### Added
- **Initial Release:** Launched the core Backend API for the Room Reservation System.
- **Room & Reservation Management:** Implemented full CRUD endpoints (Create, Read, Update, Soft Delete) for Rooms and Reservations.
- **Advanced Querying:** Added `RoomParams` and `ReservationParams` support for server-side Pagination, Searching (Name/Location), Filtering (Capacity), and Sorting.
- **Service Layer:** Introduced `IReservationService` and `ReservationService` to handle domain logic (e.g., availability checks).
- **Database:** Configured Microsoft SQL Server connection with Entity Framework Core and applied initial migrations.
- **Validation:** Implemented robust input validation using Data Annotations on DTOs.
- **Architecture:** Established the project structure with Controllers, Models, DTOs, Services, and Helpers.
- **Documentation:** Added Swagger/OpenAPI support for API testing and a comprehensive README.

### Fixed
- Addressed initial project setup and dependency injection configurations.