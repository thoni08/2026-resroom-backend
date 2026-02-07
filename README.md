# ResRoom - Backend API

## Description
This repository contains the backend REST API for **ResRoom**. The system is designed to centralize the management of campus facilities, allowing administrators to manage room data and users to view availability and make reservations.

This project solves the issue of manual and decentralized recording by providing a structured, database-driven solution for tracking room usage and status.

## Features
* **Master Room Management**: Full CRUD capabilities for managing room data (Name, Capacity, Location).
* **Soft Delete**: Implements data safety by marking records as deleted instead of physically removing them from the database.
* **RESTful Architecture**: Follows standard HTTP methods and status codes.
* **Database Migrations**: Managed via Entity Framework Core.

## Tech Stack
* **Framework**: .NET 10 (ASP.NET Core Web API)
* **Database**: Microsoft SQL Server
* **ORM**: Entity Framework Core
* **Documentation**: Swagger UI / OpenAPI

## Prerequisites
Before you begin, ensure you have met the following requirements:
* [.NET SDK 10.0](https://dotnet.microsoft.com/download) installed.
* [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) installed and set up.
* Git installed.

## Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/thoni08/2026-resroom-backend.git
    cd 2026-resroom-backend
    ```

2.  **Restore dependencies**
    ```bash
    dotnet restore
    ```

3.  **Set up Environment Variables**
    Copy the example environment file:
    ```bash
    cp .env.example .env
    ```
    *Note: Do not commit your actual `.env` file to the repository.*

4.  **Configure Database**
    Update your `.env` with your Microsoft SQL Server credentials:
    ```bash
    CONNECTION_STRINGS=Server=localhost,1433;Database=ResRoom;User Id=username;Password=Password123_;TrustServerCertificate=True;
    ```

5.  **Run Migrations**
    Apply the database schema:
    ```bash
    dotnet ef database update
    ```

## Usage

1.  **Run the Application**
    ```bash
    dotnet run
    ```
    You can also run the application in development environment (enables hot-reload)
    ```bash
    dotnet watch
    ```

2.  **Access API Documentation**
    If you run in a development environment, open your browser and navigate to the Swagger UI to test endpoints:
    ```
    http://localhost:5xxx/swagger
    ```

## Environment Variables
The application requires the following environment variables to be set in `.env` or `appsettings.Development.json`:

| Variable | Description |
| :--- | :--- |
| `CONNECTION_STRINGS` | Microsoft SQL Server connection string |
| `ASPNETCORE_ENVIRONMENT` | Set to `Development` for local testing |

## Project Structure
This project follows the **Clean Architecture** principles within a monolithic API structure:
├── Controllers/ # API Endpoints
├── Models/ # Database Entities
├── DTOs/ # Data Transfer Objects for API requests/responses & Input Validation
├── Data/ # DbContext and Config
├── Migrations/ # EF Core Migrations
└── Program.cs # App Entry Point & DI Config

## Contributing
1.  Clone the repository.
2.  Create a new branch: `git checkout -b feature/your-feature-name`
3.  Commit your changes following **Conventional Commits**: `git commit -m "feat: add new room property"`.
4.  Push to the branch: `git push origin feature/your-feature-name`.
5.  Submit a pull request.

## License
This project is licensed under the MIT License.