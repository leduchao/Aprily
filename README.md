# Aprily

Aprily is a chat-style application with an ASP.NET Core backend, PostgreSQL
database, and a React/Vite client. The repository currently contains one
backend project, one client project, and a small xUnit test project.

## Current Status

- Backend user sign-up, sign-in, sign-out, and profile lookup endpoints
- JWT bearer authentication with refresh tokens stored in an HTTP-only cookie
- PostgreSQL schema for users and refresh tokens
- EF Core `AppDbContext` plus Dapper-ready read connection factory
- React client with mobile-oriented home, login, and thread detail screens
- Client thread/message data is currently local mock data
- Chat backend is only scaffolded; no chat routes are mapped yet

## Technology

### Backend

- .NET 10 and ASP.NET Core Minimal APIs
- MediatR
- FluentValidation
- Entity Framework Core 10
- Dapper
- PostgreSQL through Npgsql
- JWT bearer authentication
- xUnit

### Client

- React 19 and TypeScript
- Vite
- TanStack Router and TanStack Query
- Tailwind CSS 4
- shadcn/ui-style components with Radix UI primitives
- React Hook Form and Zod
- Zustand
- pnpm

## Repository Structure

```text
Aprily.slnx
README.md
src/
  Aprily.Backend/
    Common/          shared constants, options, results, exceptions, extensions
    Database/        DbContext, connection factory, interceptor, init SQL
    Entities/        User, RefreshToken, AuditableEntity
    Features/
      Users/         auth, profile endpoints, user services
      Chat/          chat endpoint group placeholder
  Aprily.Client/
    src/
      components/    layout, home, thread detail, ui components
      data/          local mock thread data
      pages/         home, login, thread detail pages
      routes/        TanStack Router file routes
tests/
  Aprily.Test/       backend unit tests
```

## Prerequisites

- .NET SDK 10
- PostgreSQL
- Node.js
- pnpm
- `dotnet-ef` 10, if you want to create migrations or update the database with EF

Install the EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

## Local Setup

### 1. Configure PostgreSQL

Development settings are in:

```text
src/Aprily.Backend/appsettings.Development.json
```

The checked-in development connection strings expect:

```text
Host=localhost
Port=5432
Database=aprily-db
Username=postgres
Password=postgres
```

Create the database first. In PostgreSQL, `CREATE DATABASE` must be run by
itself, outside a transaction:

```sql
CREATE DATABASE "aprily-db";
```

Then connect to `"aprily-db"` and run:

```text
src/Aprily.Backend/Database/InitDatabase.sql
```

`InitDatabase.sql` creates the `users` and `refresh_tokens` tables. PostgreSQL
does not support MySQL-style `USE database`, so make sure your SQL client is
connected to `aprily-db` before running the table script.

### 2. Restore the Backend

From the repository root:

```bash
dotnet restore Aprily.slnx
```

### 3. Run the Backend

```bash
dotnet run --project src/Aprily.Backend/Aprily.Backend.csproj --launch-profile http
```

The HTTP profile listens on:

```text
http://localhost:5113
```

In Development, the OpenAPI document is available at:

```text
http://localhost:5113/openapi/v1.json
```

### 4. Run the Client

```bash
cd src/Aprily.Client
pnpm install
pnpm dev
```

Vite will print the local client URL, normally:

```text
http://localhost:5173
```

## Backend API

All versioned REST endpoints use the `/api/v1` prefix.

| Method | Route | Authentication |
| --- | --- | --- |
| `POST` | `/api/v1/users/auth/sign-up` | Anonymous |
| `POST` | `/api/v1/users/auth/sign-in` | Anonymous |
| `POST` | `/api/v1/users/auth/sign-out` | Bearer token |
| `GET` | `/api/v1/users/{userId}` | Anonymous |

Sign-up request:

```json
{
  "fullName": "April Ly",
  "username": "aprily",
  "email": "aprily@example.com",
  "password": "your-password"
}
```

Sign-in request:

```json
{
  "email": "aprily@example.com",
  "password": "your-password"
}
```

Sign-up and sign-in return an access token in the response body and set a
`refreshToken` cookie.

## Client Routes

| Route | Screen |
| --- | --- |
| `/` | Home thread list |
| `/login` | Login page |
| `/threads/$threadId` | Thread detail page |

The current client UI uses local data from:

```text
src/Aprily.Client/src/data/threads.ts
```

## Useful Commands

Backend:

```bash
dotnet build Aprily.slnx
dotnet test Aprily.slnx
dotnet run --project src/Aprily.Backend/Aprily.Backend.csproj --launch-profile http
```

Client:

```bash
cd src/Aprily.Client
pnpm dev
pnpm build
pnpm lint
pnpm typecheck
pnpm format
```

EF Core migration commands are also documented in:

```text
src/Aprily.Backend/MIGRATION.md
```

Db-first scaffold notes are in:

```text
src/Aprily.Backend/SCAFFOLD.md
```

## Docker Setup

You can run the full app with Docker Compose from the repository root:

```bash
docker compose up --build
```

Then open:

```text
http://localhost:8080
```

Compose starts three services:

| Service | Purpose | Host URL |
| --- | --- | --- |
| `client` | Built React/Vite app served by nginx | `http://localhost:8080` |
| `backend` | ASP.NET Core API | `http://localhost:5113` |
| `postgres` | PostgreSQL database | `localhost:5432` |

The client container proxies `/api` and `/hubs` to the backend container, so
the browser can use the same origin for API calls, cookies, and SignalR.

Useful Docker commands:

```bash
docker compose up -d --build
docker compose logs -f
docker compose down
```

To remove the persisted PostgreSQL data and uploaded files too:

```bash
docker compose down -v
```
