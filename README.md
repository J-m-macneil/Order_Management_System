# Back. Order Management System

[Live demo](https://back.software)

Back. is a full-stack order management application built around chemical distribution workflows. It supports operational order handling from customer and product management through to document generation, audit history and background processing.

The public demo includes an **Explore Demo** option on the login page. It provides read-only access so visitors can safely explore the application without changing data.

## Highlights

- Customer, contact, billing address and delivery address management.
- Product, pricing, hazard and safety data sheet management.
- Order entry with resolved pricing, discounts, special instructions and delivery details.
- Workflow states from draft through review, fulfilment, dispatch, completion, failure and cancellation.
- Background processing for order summaries, SDS bundles and simulated logistics-provider work.
- Visible failed jobs with controlled retries and audit history.
- Operational dashboard reporting for order status, priority orders, recent failures and top customers.
- Role-based access control, secure cookie authentication and a read-only demo role.
- Responsive light and dark themes.

## Technology

- **Frontend:** Angular, TypeScript, Tailwind CSS, RxJS and Lucide icons.
- **Backend:** C#, .NET 10, ASP.NET Core, Entity Framework Core and MediatR.
- **Data:** PostgreSQL with the Npgsql Entity Framework provider.
- **Documents:** QuestPDF for order summaries and SDS bundles.
- **Testing:** xUnit unit and integration tests, plus Angular tests with Vitest.
- **Deployment:** Docker Compose, Nginx and Caddy with HTTPS.

## Architecture

The backend follows a layered structure:

```text
Domain             Core entities and repository contracts
Application        Use cases, commands, queries and business rules
Infrastructure     PostgreSQL persistence, documents, identity and job processing
Server             API controllers, middleware and application composition
Client             Angular frontend
```

## Run Locally With Docker

### Prerequisites

- Docker Desktop
- A copy of the repository

### Configuration

Create a local environment file from the safe example:

```powershell
Copy-Item .env.example .env
```

Update the placeholder values in `.env`, especially:

| Variable | Purpose |
| --- | --- |
| `POSTGRES_PASSWORD` | Password for the local PostgreSQL database. |
| `JWT_SECRET_KEY` | Long random signing key for JWT authentication. |
| `APP_DOMAIN` | Domain used by Caddy. Use `localhost` locally. |

Keep the real `.env` file private. It is intentionally ignored by Git.

### Start the application

```powershell
docker compose up --build
```

The stack starts PostgreSQL, the .NET API, the Angular client and the Caddy reverse proxy. Open the local address shown in the Caddy logs, typically `https://localhost`.

## Local Development

For local development without Docker, configure the PostgreSQL connection string and JWT signing key with .NET user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=back_order_management;Username=back_app;Password=your-local-password" --project .\Order_Management_System.Server\Server.csproj
dotnet user-secrets set "Jwt:SecretKey" "your-long-random-secret" --project .\Order_Management_System.Server\Server.csproj
```

Then run the API project and Angular client from their respective development profiles.

## Tests

```powershell
dotnet test .\Domain.UnitTests\Domain.UnitTests.csproj
dotnet test .\Application.UnitTests\Application.UnitTests.csproj
dotnet test .\Infrastructure.IntegrationTests\Infrastructure.IntegrationTests.csproj

Set-Location .\order_management_system.client
npm test -- --watch=false
```

## Security Notes

- No passwords, JWT secrets, certificates or database connection strings are committed.
- Production configuration is supplied through environment variables.
- The public demo account is enforced as read-only by the server.
- The deployment exposes only the reverse proxy on ports 80 and 443; the API and database remain internal to the Docker network.
