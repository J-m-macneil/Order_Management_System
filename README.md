# Back Order Management System

Back Order Management System is a full-stack order management application built with .NET, Angular, SQL Server, and Docker.

## Configuration

Sensitive values are not committed to the repository. For Docker-based local runs, copy the example file and provide your own values:

```powershell
Copy-Item .env.example .env
```

Required Docker environment variables:

| Variable | Purpose |
| --- | --- |
| `APP_DOMAIN` | Domain used by Caddy. Use `localhost` locally. |
| `MSSQL_SA_PASSWORD` | SQL Server container password. |
| `JWT_ISSUER` | JWT issuer value. Defaults to `Back`. |
| `JWT_AUDIENCE` | JWT audience value. Defaults to `Back.Client`. |
| `JWT_SECRET_KEY` | Long random signing key used for authentication tokens. |

For local .NET development, the Development configuration uses LocalDB by default. Override `ConnectionStrings:DefaultConnection` with user secrets or environment variables if you use a different SQL Server instance.

Example user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=Back_OrderManagement_Dev;Trusted_Connection=True;TrustServerCertificate=True" --project .\Order_Management_System.Server\Server.csproj
dotnet user-secrets set "Jwt:SecretKey" "replace-with-a-long-random-secret" --project .\Order_Management_System.Server\Server.csproj
```

## Security Notes

- Do not commit `.env`, private keys, certificates, publish profiles, or generated application documents.
- Production secrets should be supplied through the hosting platform, CI/CD secrets, or a secret manager.
- The committed `Caddyfile` uses `APP_DOMAIN` so public forks do not expose deployment-specific domains.
