# Project Overview

## What it is

A small full-stack CRUD app for a merchant to manage their customers' records (personal info + address); one of the author's first full-stack projects, built to learn the stack rather than to run in production.

## Key files / paths

| Layer | Folder | Tech |
|---|---|---|
| UI | `UI/customer_management_system` | Angular 22 (SSR via `@angular/ssr`, Express server) |
| API | `API/Customer_Management_System_API` | .NET 10 / ASP.NET Core Web API, C# |
| DB | `DB/Customer_Management_System_DB` | SQL Server (SSDT `.sqlproj`, deployed via `sqlpackage`) |

```
Customer_Management_System/
├── build.sh              # restore + build + test .NET, then build Angular
├── run.sh                # start Docker DB, deploy schema, start API + Angular dev server
├── API/
│   ├── Postman/                                     # Postman collection for manual API testing
│   └── Customer_Management_System_API/
│       ├── CustomerManagementSystem.WebAPI/         # ASP.NET Core host: Program.cs, Controllers, appsettings.json
│       ├── CustomerManagementSystem.BusinessLogic/  # services, JWT creation, validation rules
│       ├── CustomerManagementSystem.DataAccess/     # ADO.NET, stored-proc calls, password hashing
│       ├── CustomerManagementSystem.Domain/         # models, config interfaces
│       └── CustomerManagementSystem.Tests/          # xUnit v3 unit tests (BusinessLogic + DataAccess, DB mocked out)
├── DB/Customer_Management_System_DB/
│   ├── Tables/                                  # tbl_customers, tbl_addresses, tbl_merchants
│   ├── Stored_Procedures/                       # usp_* — all data access goes through these
│   └── Post_Deployment_Scripts/                 # seeds the test merchant
└── UI/customer_management_system/
    └── src/app/
        ├── components/                          # one folder per route/view
        └── services/                            # HTTP calls, auth guard, session storage
```

## How it works

- Three independently-runnable layers; nothing shares process or memory — they only talk over HTTP(S)/TCP.
- Local dev DB runs as a **Docker container** (Azure SQL Edge — the only Microsoft SQL Server image with a working Apple Silicon/arm64 build). See [[local-dev-setup]].
- Data flows: Angular UI → HTTPS → ASP.NET Core API → ADO.NET (parameterized `SqlCommand`s, stored procedures only) → SQL Server.

## Gotchas / conventions

- Legacy diagrams and a `.pages` doc from the original design live in `Documentation/Diagrams/` and `Documentation/PDF/` — still broadly accurate for the JWT flow and login sequence, but written before the .NET 10 / Angular 22 modernization these docs describe.
- This is a learning project: some rough edges are deliberately left as-is rather than "fixed" — see [[known-gaps]].
