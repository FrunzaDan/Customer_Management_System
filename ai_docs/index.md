# Customer Management System — AI Docs Index

> **Read this file first**, per `learning_approach.md` and `CLAUDE.md`. It orients and links out; it does not itself contain the details — each concept lives in its own file so it can be taught, corrected, and updated independently.

## Start here

1. This file, for orientation.
2. `todo.md` — what's documented vs. pending.
3. `glossary.md` — domain terms and magic numbers (status codes, roles, acronyms) if terminology is unclear.
4. The relevant concept doc below, before exploring source directly.

## Documented Concepts

**Project shape**
- [project-overview](project-overview.md) — what this app is, the three layers, repo layout.
- [known-gaps](known-gaps.md) — deliberate limitations; read before "fixing" something that's actually known.

**Running it**
- [local-dev-setup](local-dev-setup.md) — Docker SQL Edge, `build.sh`/`run.sh`, test login, TLS-trust gotchas.

**API**
- [api-request-pipeline](api-request-pipeline.md) — middleware order, CORS, controllers, uniform response shape.
- [customer-data-model-and-lifecycle](customer-data-model-and-lifecycle.md) — tables, stored procedures, status-code lifecycle.
- [api-validation](api-validation.md) — where request validation happens before the DB is touched.
- [xunit-v3-testing](xunit-v3-testing.md) — non-obvious xUnit v3 / .NET 10 SDK test setup.

**Security**
- [jwt-auth-flow](jwt-auth-flow.md) — token issuing, claims, and the single validation path.
- [password-security](password-security.md) — PBKDF2 hashing/verification.

**Angular UI**
- [angular-app-config-and-routing](angular-app-config-and-routing.md) — HTTP client setup, the auth guard, the global 401 interceptor.
- [angular-login-flow](angular-login-flow.md) — login submission and error-message mapping.
- [angular-components-and-services](angular-components-and-services.md) — route/component map, services, fixed bugs worth not reintroducing.

## Other references in this repo

- `Documentation/Diagrams/` and `Documentation/PDF/` — legacy diagrams from the original design; broadly accurate for the JWT/login flow but predate the .NET 10 / Angular 22 modernization these docs describe. Prefer the concept docs above when they disagree.
- `API/Postman/` — a Postman collection for manual API testing.
