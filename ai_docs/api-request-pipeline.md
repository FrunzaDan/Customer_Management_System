# API Request Pipeline

## What it is

How an HTTP request moves through the ASP.NET Core API: middleware order, CORS, and the controller surface.

## Key files / paths

- `API/.../CustomerManagementSystem.WebAPI/Program.cs` — host setup, middleware pipeline.
- `API/.../CustomerManagementSystem.WebAPI/Controllers/AuthenticationController.cs`
- `API/.../CustomerManagementSystem.WebAPI/Controllers/CustomerController.cs`
- `API/.../CustomerManagementSystem.DataAccess/DBConnection/DbHelper.cs` — maps stored-proc result sets to `ResponseModel<object>`.

## How it works

Layering: `WebAPI` (controllers/host) → `BusinessLogic` (services, validation, JWT — see [[api-validation]], [[jwt-auth-flow]]) → `DataAccess` (ADO.NET + stored procs) → `Domain` (models/config). All customer/merchant DB access goes through **stored procedures** — no inline SQL, no ORM.

**Middleware order in `Program.cs`** (order matters):
`UseExceptionHandler` → `UseCors` → `UseHttpsRedirection` → `UseRateLimiter` → `UseAuthentication` → `UseAuthorization` → `MapControllers`. In non-Development environments, `UseHsts()` runs alongside `UseHttpsRedirection`.

- A global exception handler middleware catches any unhandled exception, logs it, and returns a generic `{ Message, Details }` JSON 500 (`Details` only populated in Development) — controllers themselves don't have try/catch blocks.
- CORS is locked to `Cors:AllowedOrigins` in `appsettings.json` (`http://localhost:4200`, `https://localhost:4200`), methods limited to `GET/POST/PATCH/DELETE`, headers limited to `Content-Type`/`Authorization`. No `AllowCredentials()` — consistent with bearer-token (not cookie) auth.
- Swagger UI is only wired up in Development, with a Bearer-JWT security scheme so tokens can be pasted in for manual testing.
- `AddRateLimiter` registers one named policy, `"login"`: a per-client-IP fixed-window limiter (5 requests/minute, in-memory), applied via `[EnableRateLimiting("login")]` on just `AuthenticationController.GetAccessToken` — not global, so it never throttles `verify-token` or any `CustomerController` endpoint. Exceeding it short-circuits with `429` and a small hand-written JSON body, configured via `options.OnRejected` (mirrors the exception handler's `{ Message }` shape rather than going through `ResponseModel`, since this runs before MVC's formatters). See [[known-gaps]] for why it's in-memory/per-instance rather than persistent.

**Controllers:**
- `AuthenticationController` (`api/Authentication`):
  - `POST /access-token` — body `{ merchantId, merchantPassword }` → JWT if credentials check out. Rate-limited (see above).
  - `GET /verify-token` — `[Authorize]`-gated; if the request gets past the JWT middleware, the token is valid — the endpoint has nothing left to do but return 200.
- `CustomerController` (`api/Customer`) — class-level `[Authorize]`, every endpoint requires a bearer token:
  - `POST /register`, `GET /get?searchVariable=...`, `GET /all`, `GET /export`, `GET /auditLog?customerGuid=...`, `GET /auditLog/all`, `PATCH /edit`, `PATCH /deactivate?customerGuid=...`, `PATCH /reactivate?customerGuid=...`, `DELETE /delete?customerGuid=...`.
  - `GET /export` is the one endpoint that doesn't return the uniform `ResponseModel` JSON shape below — on success it returns a raw `text/csv` `File` result instead (see [[customer-data-model-and-lifecycle]]); on failure it still returns `StatusCode(response.Status, response)` like everything else.

**Uniform response shape:** every mutating stored proc returns a `(result INT, message NVARCHAR)` result set (`result = 0` means success; nonzero mirrors an HTTP status). `DbHelper.HandleResponseWithMessage` reads that into a `ResponseModel<object>`, and every controller action passes it straight through via `StatusCode(response.Status ?? 200, response)` — controllers never branch on status themselves.

## Gotchas / conventions

- Don't add try/catch in controllers — the global exception handler is the intended single place for that.
- `DbHelper.MapCustomerFromReader` reads `reader["birthDate"]` while the actual column is `birthdate` (lowercase, per `tbl_customers`) — works because `SqlDataReader`'s name lookup is case-insensitive, but it's an inconsistency worth knowing about if this code is ever ported to a case-sensitive reader/provider.
