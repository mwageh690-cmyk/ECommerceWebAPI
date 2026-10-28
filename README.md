# E-Commerce Web API

An e-commerce backend API built with **C# and ASP.NET Core 8**, organized using a three-tier architecture. The project is being developed to support core e-commerce operations and provide backend services for web or desktop applications.

**Project status:** In Development

**Repository:** [ECommerceWebAPI](https://github.com/mwageh690-cmyk/ECommerceWebAPI)

## Overview

The goal of this project is to develop a RESTful API for an e-commerce system, with a focus on backend development, separation of responsibilities, and database integration.

The planned scope includes products, customers, orders, payments, shipping, and reviews. Features and endpoints are being implemented and refined as development progresses.

## Technology Stack

- **Language:** C#
- **Framework:** ASP.NET Core 8 Web API
- **Architecture:** Three-Tier Architecture
- **API Documentation:** Swagger / OpenAPI
- **Authentication:** JWT Bearer authentication
- **Database:** Microsoft SQL Server
- **IDE:** Visual Studio

## Architecture

The solution is organized into three main layers:

### 1. Presentation Layer

**Project:** `ECommerceAPIs`

Responsible for exposing HTTP endpoints through API controllers and handling incoming API requests.

### 2. Business Logic Layer

**Project:** `E_CommerceBussinessLayer`

Responsible for business rules and application logic, keeping business responsibilities separate from HTTP handling and data access.

### 3. Data Access Layer

**Project:** `E_CommerceDataAccessLayer`

Responsible for database access and persistence operations.

### Request Flow

```text
Client Application
       |
       v
Presentation Layer
       |
       v
Business Logic Layer
       |
       v
Data Access Layer
       |
       v
Microsoft SQL Server
```

This separation makes the responsibilities of each layer clearer and helps organize the application as it grows.

## Project Structure

```text
ECommerceWebAPI/
├── Controllers/
├── E_CommerceBussinessLayer/
├── E_CommerceDataAccessLayer/
├── Properties/
├── ECommerceAPIs.csproj
├── ECommerceAPIs.sln
├── ECommerceAPIs.http
├── Program.cs
└── README.md
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Microsoft SQL Server
- Visual Studio 2022 or another compatible .NET development environment
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/mwageh690-cmyk/ECommerceWebAPI.git
cd ECommerceWebAPI
```

### 2. Open the Solution

Open `ECommerceAPIs.sln` in Visual Studio and restore the NuGet packages.

### 3. Configure the Database

Configure the SQL Server connection string using a secure local configuration method.

Before running the application, ensure that the database configuration matches the key expected by the data access layer and that the required database schema exists.

**Security:** Never commit database passwords, JWT signing keys, or other secrets to source control.

### 4. Build the Application

From the solution directory, run:

```bash
dotnet restore ECommerceAPIs.sln
dotnet build ECommerceAPIs.sln
```

### 5. Run the API

From the solution directory, run:

```bash
dotnet run --project ECommerceAPIs.csproj
```

If the application starts successfully in the Development environment, open the Swagger URL shown in the application output or configured in `Properties/launchSettings.json`.

> These instructions describe the intended local setup. Verify the database configuration and build successfully before considering the setup complete.

## API Documentation

Swagger / OpenAPI is configured to provide an interface for exploring and testing the API during development.

Once the application is running, use Swagger to inspect the available controllers, HTTP methods, request parameters, and response schemas.

The endpoint reference will be expanded as the API implementation progresses.

## Development Roadmap

The intended project scope includes:

- Product management
- Customer management
- Order management
- Payment-related operations
- Shipping and order tracking
- Product reviews and customer feedback

These items describe the project's scope, not a guarantee that every feature is currently implemented.

## Current Status and Limitations

This project is actively being developed. API coverage, validation, security, database configuration, and testing should be reviewed as implementation progresses.

The repository documentation will be updated to reflect verified features and changes.

## Author

**Mohamed Wageh**

- GitHub: [mwageh690-cmyk](https://github.com/mwageh690-cmyk)

