# VantageView

A full-stack **Corporate News System** built with .NET—a RESTful API and Blazor frontends for publishing and managing corporate news articles.

## Overview

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
├── VantageView.API/          # REST API — public article endpoints, admin CRUD
├── VantageView.Auth/         # Auth service — JWT token issuance (admin/admin)
├── VantageView.Data/         # EF Core DbContext, Article entity, migrations
├── VantageView.Frontend/     # Public Blazor app — article listing & detail
├── VantageView.Admin.Portal/ # Admin Blazor app — article management (JWT)
└── VantageView.slnx          # Solution file
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later (project targets .NET 10)
- SQLite (no separate installation needed; EF Core SQLite provider)

## Setup & Run

### 1. Apply EF Core Migrations (optional)

Migrations are applied automatically when the API starts. To run manually:

```bash
cd src
dotnet ef database update --project VantageView.Data --startup-project VantageView.API
```

### 2. Run All Applications (recommended)

Use the **`http`** launch profile for all projects to avoid port conflicts and SSL setup.

```bash
./scripts/run-all.sh
```

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

