# VantageView

A full-stack **Corporate News System** built with .NET—a RESTful API and Blazor frontends for publishing and managing corporate news articles.

## Overview

Test stop push

VantageView demonstrates a scalable C# application with database interactions, secure authentication, and a modern UI. The system consists of:

- **Public Frontend** — Browse news articles (list view and detail pages)
- **Admin Portal** — Create, update, and delete articles (protected by JWT/API Key auth)
- **REST API** — Serves article data and exposes admin CRUD endpoints
- **Auth Service** — JWT token issuance for admin authentication

## Functional Requirements

### Public Facing (Frontend)

- **Home Page** — List of news articles with:
  - Title
  - Summary
  - Publish date
  - Author
- **Article Detail Page** — Full content of a selected article

### Admin Portal

- **Authentication** — JWT
- **Article Management** — Create, Update, Delete news articles

## Technical Stack

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core 6+ (Web API) |
| Language | C# |
| Database | SQLite (recommended) |
| ORM | Entity Framework Core (Code-First migrations) |
| Frontend | Blazor (Server or WASM) |
| API Docs | Swagger/OpenAPI |

## Project Structure

```
src/
├── VantageView.API/          # REST API — public article endpoints, admin CRUD, health checks
├── VantageView.API.Tests/    # Unit tests (e.g. HealthController)
├── VantageView.Auth/         # Auth service — JWT token issuance (admin/admin)
├── VantageView.Data/         # EF Core DbContext, entities, migrations
├── VantageView.Frontend/     # Public Blazor app — article listing & detail
├── VantageView.Admin.Portal/ # Admin Blazor app — article management (JWT)
└── VantageView.slnx          # Solution file
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later (project targets .NET 10)
- SQLite (no separate installation needed; EF Core SQLite provider)

## Setup & Run

### 1. Apply EF Core Migrations (optional)

Note: Ensure you have `dotnet ef` installed, if not run `dotnet tool install --global dotnet-ef`

## To add a new table to the migrations run:

```bash
dotnet ef migrations add ArticleHistoryNoForeignKey
```

To apply migrations to the database if the API did not automatically apply it:

```bash
cd src/VantageView.Data
dotnet ef database update
```

### 2. Run All Applications (recommended)

Use the **`http`** launch profile for all projects to avoid port conflicts and SSL setup.

**macOS / Linux:** Ensure the script is executable, then run:

```bash
chmod +x scripts/run-all.sh
./scripts/run-all.sh
```

**Windows (PowerShell):**

```powershell
.\scripts\run-all.ps1
```

> If you get an execution policy error, run: `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`

This starts:

| Application   | URL                      | Port |
|---------------|--------------------------|------|
| API           | http://localhost:5157    | 5157 |
| Auth          | http://localhost:5098    | 5098 |
| Frontend      | http://localhost:5059    | 5059 |
| Admin Portal  | http://localhost:5046    | 5046 |

Press **Ctrl+C** to stop all applications.

### 3. Run Individually

If you prefer to run each application separately, use the `http` profile so ports match the run-all script:

```bash
cd src

# API (5157)
dotnet run --project VantageView.API --launch-profile http

# Auth (5098)
dotnet run --project VantageView.Auth --launch-profile http

# Frontend (5059)
dotnet run --project VantageView.Frontend --launch-profile http

# Admin Portal (5046)
dotnet run --project VantageView.Admin.Portal --launch-profile http
```

API runs Swagger at `http://localhost:5157/swagger`. Sample articles are seeded when the database is empty. Log in to the Admin Portal at `http://localhost:5046/login` with **admin** / **admin**, then manage articles at `/articles`.

### 4. Run Unit Tests

From the repository root:

```bash
cd src
dotnet test VantageView.API.Tests/VantageView.API.Tests.csproj
```

To run tests for the whole solution:

```bash
cd src
dotnet test VantageView.slnx
```

### 5. CI / GitHub Actions

A GitHub Action (`.github/workflows/dotnet-desktop.yml`) builds the solution on every **push** and **pull request** to the `develop` branch. It restores and builds the solution; ensure tests pass locally before pushing.

