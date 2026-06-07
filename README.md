# Aprily

Aprily is a mobile-first chat application with an ASP.NET Core backend and a
React web client. The backend currently supports user authentication and
real-time, user-to-user messaging through REST endpoints and SignalR.

## Current Status

- User sign-up, sign-in, token refresh, sign-out, and profile lookup
- JWT authentication with HTTP-only refresh-token cookies
- Direct conversations and persisted message history
- Real-time direct-message delivery through SignalR
- Cursor-based pagination for conversations and messages
- English and Vietnamese web UI
- Mobile-focused responsive interface

Authentication in `Aprily.Web` is connected to the API. The current
conversation and thread screens still use mock data and have not yet been
connected to the chat REST endpoints or SignalR hub.

## Known Limitations

- The web chat screens still read from `src/Aprily.Web/src/data/chat.ts`.
- The web project does not yet include a SignalR JavaScript client.
- `pnpm lint` currently reports existing React hook, fast-refresh, and unused
  symbol errors.
- `pnpm build` currently fails on unused route components and an unused theme
  import.

## Technology

### Backend

- .NET 10 and ASP.NET Core Minimal APIs
- MediatR with command/query handlers
- FluentValidation
- Entity Framework Core 10
- PostgreSQL through Npgsql
- SignalR
- JWT bearer authentication
- xUnit

### Web

- React 19 and TypeScript
- Vite
- Material UI
- TanStack Router and TanStack Query
- Zustand
- React Hook Form and Zod
- i18next
- pnpm

## Repository Structure

```text
Aprily.slnx
src/
  Aprily.Api/             HTTP endpoints, SignalR hubs, middleware
  Aprily.Application/     Commands, queries, handlers, validation, abstractions
  Aprily.Domain/          Domain entities
  Aprily.Infrastructure/  EF Core, PostgreSQL, repositories, JWT services
  Aprily.SharedKernel/    Shared entities and Result/Error types
  Aprily.Web/             React and Vite web application
tests/
  Aprily.Test/            Backend tests
```

The .NET solution does not include `Aprily.Web`; run the web application
separately with pnpm.

## Prerequisites

- .NET SDK 10
- PostgreSQL
- Node.js 20 or newer
- pnpm
- `dotnet-ef` 10 for migrations

Install the EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

## Local Setup

### 1. Configure PostgreSQL and JWT

Development configuration is read from:

```text
src/Aprily.Api/appsettings.Development.json
```

The checked-in development connection expects:

```text
Host=localhost
Port=5432
Database=aprily
Username=postgres
Password=postgres
```

Update `ConnectionStrings:Default` and the `Jwt` settings for your local
environment. Use user secrets or environment variables for real credentials;
do not reuse the development JWT secret in production.

Equivalent environment variable names include:

```bash
ConnectionStrings__Default="Host=localhost;Port=5432;Database=aprily;Username=postgres;Password=postgres"
Jwt__Secret="replace-with-a-long-random-secret"
Jwt__Issuer="aprily"
Jwt__Audience="aprily"
Jwt__ExpirationInMinutes="10"
```

### 2. Restore and migrate the backend

From the repository root:

```bash
dotnet restore Aprily.slnx

dotnet ef database update \
  --project src/Aprily.Infrastructure/Aprily.Infrastructure.csproj \
  --startup-project src/Aprily.Api/Aprily.Api.csproj
```

### 3. Run the API

```bash
dotnet run --project src/Aprily.Api/Aprily.Api.csproj --launch-profile http
```

The HTTP profile listens at:

```text
http://localhost:5016
```

In Development, the OpenAPI document is available at:

```text
http://localhost:5016/openapi/v1.json
```

### 4. Configure and run the web client

```bash
cd src/Aprily.Web
cp .env.example .env
```

Set the local API base URL in `.env`:

```bash
VITE_API_BASE_URL=http://localhost:5016/api
```

The client appends `/v1` to this value when making requests.

Install dependencies and start Vite:

```bash
pnpm install
pnpm dev
```

The frontend runs at `http://localhost:5173`, which is the origin currently
allowed by the API CORS policy.

## API

All versioned REST endpoints use the `/api/v1` prefix.

### Authentication and Users

| Method | Route | Authentication |
| --- | --- | --- |
| `POST` | `/api/v1/users/auth/sign-up` | Anonymous |
| `POST` | `/api/v1/users/auth/sign-in` | Anonymous |
| `POST` | `/api/v1/users/auth/refresh-token` | Refresh-token cookie |
| `POST` | `/api/v1/users/auth/sign-out` | Bearer token |
| `GET` | `/api/v1/users/get-user-profile?email=...` | Bearer token |

The access token is returned in the response body. The refresh token is stored
in a secure, HTTP-only cookie.

### Direct Chat

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/chat/conversations` | List direct conversations |
| `GET` | `/api/v1/chat/direct-messages/{otherUserId}` | Load message history |
| `POST` | `/api/v1/chat/direct-messages` | Send a direct message |

Chat endpoints require a bearer token.

The conversation and history endpoints accept:

- `take`: page size from 1 to 100
- `before`: an ISO 8601 UTC timestamp used as the pagination cursor

Example send request:

```http
POST /api/v1/chat/direct-messages
Authorization: Bearer <access-token>
Content-Type: application/json

{
  "recipientUserId": "00000000-0000-0000-0000-000000000000",
  "content": "Hello from Aprily"
}
```

Additional ready-to-run examples are in
`src/Aprily.Api/Aprily.Api.http`.

## SignalR

The authenticated chat hub is available at:

```text
/hubs/chat
```

JWT authentication for WebSockets uses SignalR's `access_token` query
parameter. A JavaScript client should normally provide it with
`accessTokenFactory`.

Hub methods:

| Method | Arguments |
| --- | --- |
| `SendDirectMessage` | `recipientUserId`, `content` |
| `GetDirectMessages` | `otherUserId`, `take`, `before` |

Client event:

| Event | Payload |
| --- | --- |
| `ReceiveDirectMessage` | `ChatMessageResponse` |

Messages sent through the REST endpoint are also published to connected
SignalR clients.

## Database Migrations

Create a migration from the repository root:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Aprily.Infrastructure/Aprily.Infrastructure.csproj \
  --startup-project src/Aprily.Api/Aprily.Api.csproj \
  --output-dir Database/Migrations
```

Apply migrations:

```bash
dotnet ef database update \
  --project src/Aprily.Infrastructure/Aprily.Infrastructure.csproj \
  --startup-project src/Aprily.Api/Aprily.Api.csproj
```

## Verification

Build and test the backend:

```bash
dotnet build Aprily.slnx
dotnet test tests/Aprily.Test/Aprily.Test.csproj
```

Check the web client:

```bash
cd src/Aprily.Web
pnpm lint
pnpm build
```

The backend build currently succeeds. The web checks are the desired
verification commands, but they currently fail for the issues listed under
Known Limitations.

## Development Notes

- API commands and queries live in `Aprily.Application`.
- API endpoints and SignalR hubs should remain thin transport adapters.
- EF Core access belongs in `Aprily.Infrastructure` repositories.
- Public user, thread, and message identifiers are GUIDs; integer IDs remain
  internal database keys.
- The access token is persisted by the current web client in Zustand-backed
  local storage, while the refresh token is managed as an HTTP-only cookie.
