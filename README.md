# EsportsAPI

A RESTful esports tournament management API built with ASP.NET Core and EF Core. It models the full lifecycle of a tournament, from team registration through bracket seeding, with real business rules enforced at the domain level rather than left to the client.

## What it does

- Organizers create tournaments and teams.
- Teams register into tournaments, subject to real rules (capacity, duplicates, registration window).
- Tournaments move through a defined lifecycle where only legal state transitions are allowed.
- Locking a tournament closes registration and randomly assigns bracket seeds.

## Tech stack

- ASP.NET Core (.NET 10), controller-based Web API
- Entity Framework Core (code-first) with SQL Server
- SQL Server 2022 running in Docker via Docker Compose
- Scalar for interactive API documentation
- Secrets managed with .NET user-secrets and a local `.env` file, so no credentials live in source control

## Architecture

The project uses a layered architecture so each concern has one home:

- **Controllers** handle HTTP only. They read the request, call a service, and translate the result into the correct status code.
- **Services** hold the business logic and data access. Each service is defined by an interface and supplied through dependency injection, which keeps controllers depending on a contract rather than a concrete class and makes the logic unit-testable in isolation.
- **DTOs** separate the API's public shape from the internal entities, giving overposting protection and a stable contract independent of the database model.
- **EF Core DbContext** is the data layer. Entities map to tables through code-first migrations.

Domain outcomes are communicated across the service boundary with a result pattern: services return a status enum (and a DTO on success) rather than throwing exceptions or speaking HTTP, and the controller maps each outcome to the right response.

## Domain model

| Entity | Key fields | Notes |
| --- | --- | --- |
| Tournament | Id, Name, GameTitle, Status, MaxTeams, StartDate | Status drives the lifecycle state machine |
| Team | Id, Name, Tag | Competitors, reusable across tournaments |
| Registration | Id, TournamentId, TeamId, RegisteredAt, Seed | Join entity linking Team and Tournament |

Team and Tournament have a many-to-many relationship, resolved through the `Registration` join entity. Because a registration carries its own data (registration time, seed) and its own rules, it is modeled as a first-class entity rather than a hidden link table. Foreign keys are enforced at the database level by generated constraints.

`Tournament.Status` is an enum with four states:

```
Registration -> Locked -> InProgress -> Complete
```

## Business rules

**Registration rules** (checked in order before a team is registered):

- The tournament must exist and the team must exist.
- The tournament must be in `Registration` status, otherwise registration is closed.
- A team cannot register into the same tournament twice.
- Registration is rejected once the tournament reaches its `MaxTeams` capacity.

**Lifecycle transitions** are only allowed along legal paths. Each transition validates the current state before advancing, so illegal moves (for example, completing a tournament that never started) are rejected.

**Lock guards and side effects:** a tournament can only be locked with a non-empty, even number of registered teams. On a successful lock, the API randomly assigns unique bracket seeds (1..N) across the registered teams.

Outcomes map to HTTP status codes:

| Outcome | Status code |
| --- | --- |
| Success | 200 / 201 |
| Resource not found | 404 |
| Rule violation or illegal state transition | 409 |
| Invalid input (validation) | 400 |

## API endpoints

**Tournaments**

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/Tournament` | List all tournaments |
| GET | `/Tournament/{id}` | Get one tournament |
| POST | `/Tournament` | Create a tournament |
| POST | `/Tournament/{id}/lock` | Lock registration and assign seeds |
| POST | `/Tournament/{id}/start` | Move a locked tournament to in progress |
| POST | `/Tournament/{id}/complete` | Complete an in-progress tournament |

**Teams**

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/Team` | List all teams |
| GET | `/Team/{id}` | Get one team |
| POST | `/Team` | Create a team |

**Registrations**

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/Registration/{tournamentId}` | List a tournament's registrations, including seeds |
| POST | `/Registration/{tournamentId}/{teamId}` | Register a team into a tournament |

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/), which runs the SQL Server database
- The EF Core CLI tool: `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/Lucj0/EsportsAPI.git
cd EsportsAPI
```

### 2. Configure secrets

Credentials are kept out of source control, so you provide them locally.

Create a `.env` file in the project root (it is gitignored) with the SQL Server password:

```
MSSQL_SA_PASSWORD=YourStrong!Passw0rd
```

Then store the connection string in user-secrets, using the same password:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=EsportsDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
```

### 3. Start the database

```bash
docker compose up -d
```

SQL Server takes 20 to 30 seconds to finish starting the first time.

### 4. Apply migrations

```bash
dotnet ef database update
```

### 5. Run

```bash
dotnet run
```

The terminal prints the URL it is listening on (by default `http://localhost:5126`). Open that address with `/scalar` appended, for example `http://localhost:5126/scalar`, to explore and call every endpoint through the interactive API docs.