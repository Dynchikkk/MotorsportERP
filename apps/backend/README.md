# MotorsportERP Backend

Backend for Motorsport ERP built on .NET and split by layers (`Domain`, `Application`, `Infrastructure`, `WebApi`).

## Stacl

- Target framework: `net8.0` (all backend projects).
- Recommended SDK: `.NET 8 SDK` (8.0.x).

## Project layout

- `MotorsportErp.WebApi` - ASP.NET Core API host, auth, controllers, Swagger.
- `MotorsportErp.Application` - application logic, contracts, DTOs, use cases.
- `MotorsportErp.Infrastructure` - EF Core, SQL Server, repositories, external services.
- `MotorsportErp.Domain` - entities, enums, domain models.

## Local run (without Docker frontend)

From backend root:

```bash
cd apps/backend
dotnet restore
dotnet build
dotnet run --project MotorsportErp.WebApi
```

API default local URL depends on launch profile and env; in this repo Docker maps API to `http://localhost:5000`.
