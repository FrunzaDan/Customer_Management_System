# Customer Management System — Project Index

> **Purpose of this file**: a single, up-to-date reference for the whole
> project — what it is, how the three layers (UI / API / DB) fit together,
> how to run it locally, and how auth/security work. Written to be read by
> a human contributor **and** by an AI assistant picking up this repo cold —
> so it favors precise facts (file paths, field names, status codes) over
> prose. If you change behavior described here, update this file in the
> same change.

## 1. What this project is

A small full-stack CRUD app for a merchant to manage their customers'
records (personal info + address). It's one of the author's first
projects, built to learn the full stack rather than to run in production.
Three layers, each in its own top-level folder:

| Layer | Folder | Tech |
|---|---|---|
| UI | `UI/customer_management_system` | Angular 22 (SSR via `@angular/ssr`, Express server) |
| API | `API/Customer_Management_System_API` | .NET 10 / ASP.NET Core Web API, C# |
| DB | `DB/Customer_Management_System_DB` | SQL Server (SSDT `.sqlproj`, deployed via `sqlpackage`) |

Local dev DB runs as a **Docker container** (Azure SQL Edge — the only
Microsoft SQL Server image with a working Apple Silicon/arm64 build).

Legacy diagrams and a `.pages` doc from the original design live in
`Documentation/Diagrams/` and `Documentation/PDF/` — still broadly
accurate for the JWT flow and login sequence, but written before the
.NET 10 / Angular 22 modernization described here.

## 2. Repo layout

```
Customer_Management_System/
├── build.sh              # restore + build + test .NET, then build Angular
├── run.sh                # start Docker DB, deploy schema, start API + Angular dev server
├── API/Customer_Management_System_API/
│   ├── CustomerManagementSystem.WebAPI/         # ASP.NET Core host: Program.cs, Controllers, appsettings.json
│   ├── CustomerManagementSystem.BusinessLogic/  # services, JWT creation/validation, validation rules
│   ├── CustomerManagementSystem.DataAccess/     # ADO.NET, stored-proc calls, password hashing
│   ├── CustomerManagementSystem.Domain/         # models, config interfaces
│   ├── CustomerManagementSystem.Tests/          # xUnit.net v3 unit tests (BusinessLogic + DataAccess, DB mocked out)
│   └── Postman/                                 # Postman collection for manual API testing
├── DB/Customer_Management_System_DB/
│   ├── Tables/                                  # tbl_customers, tbl_addresses, tbl_merchants
│   ├── Stored_Procedures/                       # usp_* — all data access goes through these
│   └── Post_Deployment_Scripts/                 # seeds the test merchant
└── UI/customer_management_system/
    └── src/app/
        ├── components/                          # one folder per route/view
        └── services/                            # HTTP calls, auth guard, session storage
```

## 3. Running it locally

### 3.1 Docker SQL Server (Azure SQL Edge)

```bash
docker pull mcr.microsoft.com/azure-sql-edge

docker run \
  -e "ACCEPT_EULA=1" \
  -e "MSSQL_SA_PASSWORD=MyStrongPassw0rd?" \
  -p 1433:1433 \
  --name sqlserver \
  --platform linux/arm64 \
  -d mcr.microsoft.com/azure-sql-edge
```

- `sa` password for local dev is **`MyStrongPassw0rd?`** — matches
  `appsettings.json`'s `CustomerManagementSystemDB_Docker` connection
  string and `run.sh`'s `SQL_SA_PASSWORD` default. Local-dev-only
  credential, not a production secret.
- Azure SQL Edge does **not** ship `sqlcmd`/`mssql-tools` inside the
  container, so readiness can't be probed with the usual
  `docker exec ... sqlcmd` trick. `run.sh` instead retries the real
  `sqlpackage publish` (with jitter) until it succeeds.
- Container name `sqlserver`, port `1433`. `--platform linux/arm64` is
  needed on Apple Silicon Macs.

### 3.2 `build.sh` (root)

Restores, builds, and tests the .NET solution, builds the DB `.sqlproj`,
then does `npm ci && npm run build` for Angular. No live services
required — pure compile/test verification. Run this before committing
API/DB/UI changes.

### 3.3 `run.sh` (root)

Full local dev environment, idempotent (safe to re-run):
1. Starts Docker Desktop if not running, starts/creates the `sqlserver`
   container if needed.
2. Installs `sqlpackage` (pinned to `170.3.93` — newer releases can need
   a .NET runtime patch this machine doesn't have) and publishes the
   DB `.dacpac`, retrying with jitter until SQL Server accepts
   connections.
3. Starts the API in the background (`dotnet run --launch-profile https`,
   `https://localhost:7145`), waits for `/swagger/index.html` to respond.
4. Extracts the API's live TLS cert (via `openssl s_client`) into
   `.run/dev-cert.pem` and sets `NODE_EXTRA_CA_CERTS` to it, so Node's
   `fetch()` (used during Angular SSR) trusts it — see §6.3.
5. Starts the Angular dev server in the foreground (`npm start`,
   `http://localhost:4200`). Ctrl+C stops both API and Angular.

Logs land in `.run/` (gitignored, along with the exported dev cert).

**One-time local machine setup**, not handled by the scripts: the
ASP.NET Core HTTPS dev certificate must be trusted at the OS level —
`dotnet dev-certs https --trust`. If you ever see a browser
`ERR_CERT_AUTHORITY_INVALID` on `https://localhost:7145`, there are
likely stale/duplicate dev certs in the keychain; `dotnet dev-certs
https --clean && dotnet dev-certs https --trust` resolves it (see §6.3).

### 3.4 Test login

Seeded by `DB/Customer_Management_System_DB/Post_Deployment_Scripts/post_deployment_populate_tbl_merchants.sql`:

- Merchant ID: `TestMerchantID`
- Password: `Merchant123`

## 4. How the API works

`.NET 10`, layered: `WebAPI` (controllers/host) → `BusinessLogic`
(services, validation, JWT) → `DataAccess` (ADO.NET + stored procs) →
`Domain` (models/config). All customer/merchant DB access goes through
**stored procedures** — no inline SQL, no ORM.

### 4.1 Program.cs pipeline (`API/.../CustomerManagementSystem.WebAPI/Program.cs`)

Order matters here: `UseExceptionHandler` → `UseCors` →
`UseHttpsRedirection` → `UseAuthentication` → `UseAuthorization` →
`MapControllers`. A global exception handler middleware catches any
unhandled exception, logs it, and returns a generic
`{ Message, Details }` JSON 500 (Details only populated in
Development) — controllers themselves don't have try/catch blocks.

CORS is locked down to `Cors:AllowedOrigins` in `appsettings.json`
(`http://localhost:4200`, `https://localhost:4200`), methods limited to
`GET/POST/PATCH/DELETE`, headers limited to `Content-Type`/`Authorization`.

### 4.2 Controllers

- **`AuthenticationController`** (`api/Authentication`)
  - `POST /access-token` — body: `{ merchantId, merchantPassword }` →
    returns a JWT if credentials check out.
  - `GET /verify-token` — `[Authorize]`-gated; if the request gets past
    the JWT middleware, the token is valid — the endpoint has nothing
    left to do but return 200.
- **`CustomerController`** (`api/Customer`) — class-level `[Authorize]`,
  every endpoint requires a bearer token:
  - `POST /register` — create a customer.
  - `GET /get?searchVariable=...` — lookup by GUID/email/etc.
  - `GET /all` — list all customers.
  - `PATCH /edit` — update fields on an existing customer.
  - `PATCH /deactivate?customerGuid=...`
  - `PATCH /reactivate?customerGuid=...`
  - `DELETE /delete?customerGuid=...`

### 4.3 Customer lifecycle (status codes in `tbl_customers.customer_Status`)

- `1901` = **active**
- `1903` = **deactivated**

Rules enforced in the stored procedures themselves (not just the API layer):
- New customers are created **active** (`usp_createCustomer.sql`).
- `usp_deactivateCustomer` only updates rows where status `<> 1903`
  (no-op → `409` if already deactivated).
- `usp_reactivateCustomer` only updates rows where status `<> 1901`
  (no-op → `409` if already active).
- **`usp_deleteCustomer` requires status `= 1903`** — a customer must be
  deactivated first; deleting an active customer returns `409` with
  `'Customer must be deactivated before it can be deleted.'`. This was a
  deliberately-verified business rule (deactivate → delete → reactivate
  is not a valid path; only deactivate → delete or deactivate →
  reactivate).

Every mutating stored proc returns a uniform `(result INT, message
NVARCHAR)` result set (`result = 0` means success; nonzero mirrors an
HTTP status the controller passes straight through via
`StatusCode(response.Status ?? 200, response)`).

### 4.4 Data model

- **`tbl_customers`**: `PK_customer_guid` (NVARCHAR 50, app-generated —
  the API always overwrites any client-supplied GUID with a fresh
  `Guid.NewGuid()` in `CustomerRegistration.cs`, so clients can't pick
  their own IDs), `first_name`, `last_name`, `email`, `msisdn`,
  `gender` (INT: presumably 0/1/2 — see UI §5.4), `birthdate`,
  `customer_Status`, `creation_Date`, `interaction_Date`.
- **`tbl_addresses`**: 1:1 with a customer via `FK_customer_guid`
  (`ON UPDATE CASCADE`) — country/county/town/zip/street/number.
- **`tbl_merchants`**: `merchant_id`, `merchant_password` (BINARY 32 —
  PBKDF2 hash), `merchant_password_salt` (BINARY 16), `merchant_role`
  (`1801` = the only role currently in use), `last_interaction`.

### 4.5 Validation (BusinessLogic layer, before hitting the DB)

- **`CustomerEditing`**: GUID required + format-validated
  (`GuidValidation`); `Email`/`Msisdn`, if present, are format-validated
  before the edit is attempted.
- Email/MSISDN uniqueness is enforced in `usp_createCustomer` itself
  (`400` if either already exists).
- Regexes for these live client-side too, in Angular's
  `environment.ts` (`EmailRegex`, `PhoneRegex`, `UserName`) — kept in
  sync manually, not shared/generated from one source.

### 4.6 Tests (`CustomerManagementSystem.Tests`)

xUnit.net **v3** (`xunit.v3` 4.0.0, not the older `xunit` v2 meta-package),
with **Moq** for mocking `IDbUtils`/`IAppSettingsConfig`. Covers
`BusinessLogic` (validations, `JwtCreation`/`JwtValidation`,
`CustomerRegistration`/`Editing`/`Getting`/`Activation`/`Deletion`) and
`DataAccess`'s `PasswordHasher` — all pure logic, no live DB or Docker
needed, matching where `build.sh` runs `dotnet test` (before the DB/Docker
steps even happen).

Two things specific to xUnit v3 on the **.NET 10 SDK**, not present in
older xUnit v2 test projects:
- The **.NET 10 SDK dropped VSTest support for xUnit v3** entirely —
  `dotnet test` fails with "Testing with VSTest target is no longer
  supported..." unless the project opts into the new **Microsoft Testing
  Platform (MTP)** runner. That opt-in is a **repo-root `global.json`**:
  ```json
  { "test": { "runner": "Microsoft.Testing.Platform" } }
  ```
  This is discovered by walking **up from the current working
  directory** the `dotnet` command is run from — not from the project or
  `.sln` path — so it has to sit somewhere `dotnet test` will actually be
  invoked from or below. `build.sh` runs `dotnet test` from the repo
  root, hence the file living there. (This is a separate, unrelated
  `global.json` from `DB/Customer_Management_System_DB/global.json`,
  which only pins the SQL project's SDK version — the two don't
  conflict, since discovery stops at the nearest one found.)
- xUnit v3 test projects compile to a **console executable**
  (`<OutputType>Exe</OutputType>` in the `.csproj`), not a library —
  the test assembly is its own runner now, a fundamental v3 architecture
  change from v2.

## 5. How the Angular UI works (and how it talks to the API)

### 5.1 Config

`src/environments/environment.ts`:
```ts
export const environment = {
  CustomerManagementSystemAPI: 'https://localhost:7145',
  EmailRegex: "^\\S+@\\S+\\.\\S+$",
  PhoneRegex: "^[0-9]{9,12}$",
  UserName: "^[a-zA-z0-9\\ ]*$"
};
```
Every service builds its request URL off `CustomerManagementSystemAPI`.

`app.config.ts` wires up: Router (with component input binding, view
transitions, scroll restoration), **SSR client hydration**
(`provideClientHydration(withEventReplay(), withNoIncrementalHydration())`),
and `HttpClient` using the **fetch-based backend**
(`provideHttpClient(withFetch(), ...)`) — this last detail matters for
SSR (see §6.3): during server-side rendering, HTTP calls run through
Node's native `fetch()`, not a browser's.

### 5.2 Routing & the auth guard (`app.routes.ts`, `auth-guard.service.ts`)

All routes except `/login` and the catch-all 404 carry
`canActivate: [authGuardFn]`. The guard:
1. Calls `VerifyTokenService.isTokenValid()`, which hits
   `GET /api/Authentication/verify-token` with whatever token is in
   session storage.
2. `200` → allow navigation. Anything else (401, network error, TLS
   error) → clear session storage, redirect to
   `/login?sessionExpired=true`.

Because routes are guarded and the app uses SSR, **this guard's HTTP
call can run inside Node** (server-side) as well as in the browser
(client-side, post-hydration) — see §6.3 for why that has separate TLS
trust implications.

### 5.3 Login flow (`user-login.component.ts`, `user-login.service.ts`)

1. User submits Merchant ID + password → `UserLoginService.login()` →
   `POST /api/Authentication/access-token`.
2. On success (`200` + a token in the body): token is stashed via
   `SessionStorageService`, app navigates to `/customers`.
3. On error, `handleLoginError(status)` maps the HTTP status to a
   message:
   - `403` → "Merchant credentials are incorrect!"
   - `404` → "Endpoint is down!"
   - `0` (no response reached the browser at all — network/TLS-level
     failure) → "Could not reach the server. It may be offline, or
     your browser does not trust its security certificate."
   - anything else → `"Server error ({statusCode}). Please try again
     later."`

   `status === 0` is a deliberately distinct case: it's what a rejected
   TLS certificate (e.g. `ERR_CERT_AUTHORITY_INVALID`) looks like to
   Angular's `HttpClient` — no response body, no real status code — so
   the message says so instead of defaulting to a generic "server is
   down" that would be misleading (the server is up; the browser just
   doesn't trust its cert).

### 5.4 Components (`src/app/components/`)

| Component | Route | Purpose |
|---|---|---|
| `user-login` | `/login` | Merchant sign-in form |
| `home` | `/`, `/customers` | Customer list landing page |
| `customer-list` | (used by `home`) | Table of all customers, `@for` track-by GUID |
| `customer-details` | `/customerDetails` | Single customer's full record |
| `add-customer` | `/addCustomer` | Create-customer form |
| `edit-customer` | `/editCustomer` | Edit-customer form |
| `features` | `/features` | Static feature list page |
| `about` | `/about` | Static about page |
| `navigation-bar` | (shown/hidden via `NavbarService`) | Top nav — hidden on `/login` |
| `footer` | (shown/hidden via `FooterService`) | Page footer — hidden on `/login` |
| `display-error` | n/a | Reusable error display |
| `page-not-found` | `**` | 404 fallback |

Gender is stored/sent as an **integer**: `0` = Not declared, `1` = Male,
`2` = Female (canonicalized across `add-customer` and `edit-customer`
forms — both used to disagree on string vs numeric values, now fixed).

### 5.5 Services (`src/app/services/`)

- `user-login.service.ts` — login POST, session token handoff.
- `verify-token.service.ts` — calls `verify-token`, exposes
  `isTokenValid(): Observable<boolean>` via clean
  `pipe(map(() => true), catchError(() => of(false)))` — deliberately
  simple; an earlier version used a manually-managed RxJS `Subject`
  that crashed (`Cannot read properties of null`) whenever the API
  returned a 401 with an empty body, which orphaned the guard's
  subscription and cascaded into hydration timeouts and an unclickable
  login form. Don't reintroduce a manual `Subject` here.
- `auth-guard.service.ts` — route guard, described in §5.2.
- `session-storage.service.ts` — stores/reads/clears the JWT in
  `sessionStorage`.
- `http-header-service.ts` — builds request headers, attaches
  `Authorization: Bearer <token>` where needed.
- `customer-list.component.ts`'s duplicate-GUID check runs as an
  `effect()` over the `duplicateGuids` computed signal (constructor,
  not `ngOnInit`), so it re-evaluates whenever `customers()` actually
  changes — a one-time synchronous check right after the async
  `loadCustomers()` call used to always see a stale/empty list and
  never fire.
- `get-customer.service.ts`, `add-customer.service.ts`,
  `edit-customer.service.ts`, `activate-customer.service.ts`,
  `delete-customer.service.ts` — one per `CustomerController` endpoint
  group.
- `navbar.service.ts`, `footer.service.ts` — simple show/hide state for
  chrome that shouldn't appear on the login screen.

## 6. Security: JWT, password hashing, and TLS

### 6.1 JWT issuing (`JwtCreation.cs`)

- Symmetric HMAC-SHA256 signing (`SymmetricSecurityKey` +
  `HmacSha256Signature`), key from `Auth:SecureJWTKey` in
  `appsettings.json` (base64 string — **committed value is a demo
  placeholder**, not a real secret; replace it for anything beyond
  local dev).
- Claims: `ClaimTypes.Sid` = merchant ID, `Jti` = random GUID, `Iat` =
  issue time.
- Issuer/Audience both `https://localhost:7145/`, expiry from
  `Auth:AccessTokenTimeout` (currently `15` minutes).
- Issued from `POST /api/Authentication/access-token`, only after
  `DbUtils.CheckMerchantCredentialsFromDb` confirms the password.

### 6.2 JWT validation

Two places validate tokens, deliberately for different purposes:
- **`Program.cs`'s `AddJwtBearer` middleware** — the real gate. Runs on
  every `[Authorize]`-attributed endpoint (`CustomerController`
  entirely, `AuthenticationController.VerifyToken`). Checks signature,
  issuer, audience, expiry (`ClockSkew = TimeSpan.Zero`, no grace
  period).
- **`JwtValidation.ValidateToken()`** — used internally by
  `AuthService.GetAccessToken` as a self-check right after minting a
  new token (defense-in-depth: makes sure the token it just built is
  actually valid before handing it back), and backs the
  `verify-token` endpoint's implicit check. Also verifies a `Sid` claim
  is present and non-empty.

### 6.3 Password storage

`PasswordHasher.cs` (`DataAccess/DBConnection/`): **PBKDF2-HMACSHA256**,
100,000 iterations, 16-byte salt, 32-byte hash — replaced an earlier
unsalted-SHA-256 scheme. `tbl_merchants` has separate
`merchant_password` (hash) and `merchant_password_salt` columns.
`usp_getMerchantAuthData` fetches the hash+salt (and bumps
`last_interaction`); the comparison itself
(`PasswordHasher.VerifyPassword`) happens in C#, not in SQL.

### 6.4 TLS trust — a recurring local-dev gotcha

The API serves HTTPS via the ASP.NET Core **dev certificate**
(self-signed, `dotnet dev-certs https`). Two distinct trust problems
have come up here and are worth understanding before touching auth or
`run.sh` again:

1. **Browser trust** (`ERR_CERT_AUTHORITY_INVALID` in the browser,
   login fails with the UI's `status === 0` message): the browser
   rejects Kestrel's cert outright. Root cause seen in practice: running
   `dotnet dev-certs https --export-path` **regenerates** the dev cert
   rather than exporting the existing one, which desyncs "the cert
   Kestrel serves" from "the cert the OS actually trusts" (multiple
   `localhost` dev certs end up in the login keychain). Fix: `dotnet
   dev-certs https --clean && dotnet dev-certs https --trust` — trusts
   whichever cert is now current, cleanly. Never use `--export-path` to
   "just get the current cert" — it doesn't.
2. **Node/SSR trust** (silent `AbortError`s in the terminal for guarded
   routes, page still works after client-side hydration): Angular SSR
   makes real `fetch()` calls from **Node**, whose TLS stack doesn't
   consult the per-user login keychain the way a browser or `curl`
   does — so even a cert the OS trusts can fail Node's handshake.
   `run.sh` works around this by extracting the *live* served cert via
   `openssl s_client | openssl x509` and pointing `NODE_EXTRA_CA_CERTS`
   at it before starting the Angular dev server (§3.3 step 4). This is
   separate from problem #1 and fixing one does not fix the other.

## 7. Known gaps / deliberately deferred

- The committed `Auth:SecureJWTKey` is a placeholder value, fine for
  local dev, must be replaced (and pulled out of source control) for
  any real deployment.
