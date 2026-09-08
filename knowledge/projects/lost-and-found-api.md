---
title: Lost & Found API

organization: School Project

role: Backend Developer

environment: development

period:
  from: 2026-07
  to: 2026-07

status: completed

technologies:
  - csharp
  - dotnet
  - aspnet-core
  - entity-framework-core
  - postgresql
  - docker
  - xunit
  - swagger
  - dotnetenv

concepts:
  - backend-api
  - repository-pattern
  - automated-testing
  - docker-development
  - backend-validation
  - item-status-management
  - search
  - filtering
  - environment-configuration
  - utc-timestamps
  - api-documentation

dependencies:
  - Microsoft.EntityFrameworkCore
  - Npgsql.EntityFrameworkCore.PostgreSQL
  - Swashbuckle.AspNetCore
  - xunit
  - DotNetEnv

links:
  github:
  live:

---

# Overview

Built a small Lost & Found backend API using ASP.NET Core Web API and PostgreSQL.

The system allows found items to be registered and managed through an HTTP API. Items can be searched and filtered, claimed by users, returned to their owners, and deleted while their status allows it.

The project also includes automated tests, Swagger documentation, Docker-based development, and separate repository implementations for production-like database access and fast in-memory testing.

---

# Context

The project was developed as a backend-focused school project.

The goal was to build a REST-style API that persisted data in PostgreSQL while demonstrating common backend development practices such as repository abstraction, request validation, status-based business rules, automated testing, environment configuration, and API documentation.

The application needed to run together with its PostgreSQL database through Docker Compose and provide enough API functionality to manage the complete lifecycle of a found item.

---

# Task

The task was to design and implement the backend for a Lost & Found system.

My responsibilities included:

- Designing the API for found-item management.
- Implementing item creation, listing, searching, and filtering.
- Implementing claim, return, and deletion operations.
- Defining and enforcing item status rules.
- Implementing PostgreSQL persistence through Entity Framework Core.
- Creating a repository abstraction.
- Providing an in-memory repository for fast automated tests.
- Implementing API validation and error handling.
- Configuring the application and database through environment variables.
- Setting up Docker Compose for local development.
- Documenting the API through Swagger.
- Writing automated tests for core application behaviour.

---

# Challenge

## Challenge: Managing Item State

### Problem

A found item can exist in different states, and the operations available for an item depend on its current state.

For example, a newly registered item should be available for claiming, while operations such as deletion should not necessarily remain valid after the item has progressed through its lifecycle.

The rules needed to be enforced by the backend rather than relying on API clients to behave correctly.

### Solution

New items are created with `Available` status.

The backend applies status-based rules when handling claim, return, and delete operations.

These rules are covered by automated tests so that changes to the implementation do not silently alter the expected item lifecycle.

### Result

The item lifecycle is controlled by explicit backend rules instead of being left to individual API clients.

---

## Challenge: Testing Persistence-Dependent Behaviour

### Problem

The project required automated tests for application behaviour, but using a real PostgreSQL database for every test would make the test suite slower and more dependent on external infrastructure.

### Solution

A repository abstraction was introduced for persistence.

The application provides:

- A PostgreSQL repository for normal execution.
- An in-memory repository for fast automated tests.

The tests can therefore exercise repository and application behaviour without requiring PostgreSQL for every test run.

### Result

The test suite remains fast and isolated while the application still uses PostgreSQL for persistent execution.

---

## Challenge: Running the API and Database Together

### Problem

The API depends on PostgreSQL for persistent storage. Setting up the correct database environment manually would add unnecessary configuration overhead during development.

### Solution

Docker Compose was used to run the ASP.NET Core Web API and PostgreSQL database together.

Database configuration is provided through environment variables, with an example environment file supplied for local setup.

PostgreSQL data is stored in a Docker volume so that it survives container restarts.

### Result

The complete backend environment can be started consistently through Docker Compose without manually installing and configuring PostgreSQL on the development machine.

---

# Action

## Architecture

Implemented the backend as a layered ASP.NET Core application with a repository abstraction separating application logic from persistence.

### Backend

The ASP.NET Core Web API exposes endpoints for:

- Creating found items.
- Listing found items.
- Searching items.
- Filtering by status.
- Filtering by category.
- Claiming items.
- Returning items to owners.
- Deleting available items.

The API validates incoming requests and applies item status rules before performing state-changing operations.

Swagger is included for endpoint documentation and interactive API testing.

### Persistence

PostgreSQL is used for persistent application data.

Entity Framework Core provides database access, with a PostgreSQL-specific repository implementing the persistence layer.

The repository abstraction keeps persistence operations separate from the rest of the application and also allows the in-memory implementation to be used during automated tests.

Entity Framework Core `EnsureCreated()` is used to create the database schema automatically during development.

### Development Environment

Docker Compose runs the ASP.NET Core Web API and PostgreSQL database as a single development environment.

Database credentials and connection configuration are supplied through environment variables and loaded using DotNetEnv.

PostgreSQL uses a Docker volume for persistent development data.

### Testing

xUnit is used for automated testing.

The test suite covers:

- Item creation.
- Default `Available` status.
- UTC timestamps.
- Claim rules.
- Return rules.
- Delete rules.
- Repository filtering.
- API validation.
- Missing resources.
- API responses for creation, return, and deletion.

---

## Technical Decisions

### Decision: Repository Abstraction

#### Context

The application needed persistent storage while also requiring fast automated tests.

#### Chosen Solution

A repository abstraction was introduced with separate implementations for PostgreSQL and in-memory storage.

#### Alternatives Considered

The application could have accessed Entity Framework Core directly from the API layer and used the same database implementation for all tests.

#### Trade-offs

The abstraction adds an extra layer to a relatively small application, but it provides a clear separation between application behaviour and persistence and makes fast in-memory testing possible.

---

### Decision: PostgreSQL with Docker Compose

#### Context

The application required relational persistence and a reproducible local development environment.

#### Chosen Solution

PostgreSQL was selected as the database and run through Docker Compose alongside the ASP.NET Core Web API.

A Docker volume is used for database persistence.

#### Trade-offs

The Docker-based setup introduces a dependency on Docker during development, but provides a consistent API and database environment without requiring PostgreSQL to be installed directly on the host machine.

---

### Decision: In-Memory Repository for Tests

#### Context

Automated tests should run quickly and should not depend on a running PostgreSQL container.

#### Chosen Solution

An in-memory repository is used by the automated tests.

The PostgreSQL repository remains the persistence implementation for normal application execution.

#### Trade-offs

The in-memory implementation does not reproduce every behaviour of PostgreSQL, so it cannot replace database integration testing completely.

For this project, it provides a fast way to test application and repository behaviour without introducing database infrastructure into the core test suite.

---

### Decision: UTC Timestamps

#### Context

Found-item timestamps are part of the application's data and are used when reporting and testing item creation.

#### Chosen Solution

Timestamps are generated in UTC.

#### Trade-offs

UTC avoids ambiguity caused by local time zones and provides a consistent representation for persisted timestamps.

---

### Decision: Swagger

#### Context

The API needed to be easy to inspect and test during development.

#### Chosen Solution

Swagger was added to document the HTTP API and provide an interactive Swagger UI.

#### Trade-offs

Swagger adds a development dependency and generated documentation, but significantly reduces friction when manually exploring and testing the API.

---

## Implementation

The application follows a backend architecture where the ASP.NET Core Web API handles HTTP requests, application rules are separated from persistence, and repositories provide access to stored items.

The main flow is:

1. A client sends a request to the ASP.NET Core Web API.
2. The API validates the request.
3. The application applies the relevant item status and business rules.
4. Repository operations are used to retrieve or modify items.
5. Used Entity Framework Core with PostgreSQL for persistence, with an in-memory repository for automated tests.
6. API responses are returned to the client.
7. Swagger exposes the available endpoints for exploration and testing.
8. Automated tests use the in-memory repository to verify application behaviour without requiring PostgreSQL.

The development environment consists of:

- ASP.NET Core Web API.
- PostgreSQL.
- Docker Compose.
- Docker volume for PostgreSQL persistence.
- Environment variables loaded through DotNetEnv.

The API is exposed at:

`http://localhost:8080/api/items`

Swagger UI is available at:

`http://localhost:8080/swagger/index.html`

The application can be started with:

`docker compose up --build`

Tests can be executed with:

`dotnet test`

---

# Result

The completed backend provides a functional Lost & Found API with PostgreSQL persistence.

The system supports:

- Creating found items.
- Listing items.
- Searching items.
- Filtering by status and category.
- Claiming items.
- Returning items to owners.
- Deleting available items.
- Status-based business rules.
- API request validation.
- PostgreSQL persistence.
- Swagger documentation.
- Automated testing.

The application and PostgreSQL database can be started together through Docker Compose.

The test suite verifies the main item lifecycle rules, repository behaviour, validation, timestamps, missing resources, and relevant API responses.

---

# Lessons Learned

## Repository Abstraction Should Have a Purpose

The repository abstraction added structure to a small application, but it had a concrete purpose: it allowed the PostgreSQL implementation to be separated from a fast in-memory implementation used by the tests.

An abstraction is useful when it solves a real problem. Adding layers without a reason would only increase complexity.

## Business Rules Belong in the Backend

Item status rules should not depend on the client correctly deciding which operations are allowed.

The API must validate the current state and reject invalid transitions regardless of which client sends the request.

## Automated Tests Should Avoid Unnecessary Infrastructure

Using an in-memory repository made the core test suite faster and easier to run.

However, this also highlighted the distinction between unit-level application tests and database integration tests. An in-memory implementation cannot guarantee that PostgreSQL behaves identically.

## Docker Simplifies Development Environment Setup

Running the API and PostgreSQL through Docker Compose provided a reproducible development environment.

The application did not require PostgreSQL to be installed directly on the development machine.

## UTC Is a Better Default for Persisted Timestamps

Using UTC for stored timestamps avoids ambiguity between different local time zones and makes timestamp comparisons more predictable.

---

# Future Improvements

- Add dedicated integration tests against PostgreSQL to complement the in-memory repository tests.
- Replace `EnsureCreated()` with Entity Framework Core migrations for more controlled schema evolution.
- Add authentication and authorization for operations such as claiming, returning, and deleting items.
- Introduce structured error responses using a consistent API error format.
- Add pagination to item listing and search endpoints.
- Add more advanced search and filtering capabilities.
- Add API integration tests that exercise the complete HTTP-to-database flow.
- Add CI/CD to automatically build, test, and validate the application.
- Add production-oriented configuration and secret management instead of relying on local environment files.
- Add logging and observability for API errors and important item state transitions.

---