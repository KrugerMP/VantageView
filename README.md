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
├── VantageView.Auth/         # Auth service — JWT token issuance
├── VantageView.Frontend/     # Public Blazor app — article listing & detail
├── VantageView.Admin.Portal/ # Admin Blazor app — article management
└── VantageView.slnx          # Solution file
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later (minimum: .NET 6; project targets .NET 10)
- SQLite (no separate installation needed when using EF Core SQLite provider)

## Setup & Run

### 1. Apply EF Core Migrations

```bash
cd src/VantageView.API
dotnet ef database update
```

> **Note:** Migrations will be added once the EF Core DbContext and models are in place.

### 2. Run the API

```bash
cd src/VantageView.API
dotnet run
```

API will be available at `https://localhost:5xxx` (port from `launchSettings.json`). Swagger UI is enabled in Development.

### 3. Run the Auth Service (for JWT)

```bash
cd src/VantageView.Auth
dotnet run
```

### 4. Run the Public Frontend

```bash
cd src/VantageView.Frontend
dotnet run
```

### 5. Run the Admin Portal

```bash
cd src/VantageView.Admin.Portal
dotnet run
```

