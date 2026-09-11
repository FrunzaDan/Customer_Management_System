# JWT Auth Flow

## What it is

How a merchant's credentials become a bearer token, what claims that token carries, and where it gets validated on every subsequent request.

## Key files / paths

- `API/.../CustomerManagementSystem.BusinessLogic/AuthFunctions/JwtCreation.cs` — builds and signs the token.
- `API/.../CustomerManagementSystem.BusinessLogic/Services/Implementation/AuthService.cs` — orchestrates credential check → token issuance.
- `API/.../CustomerManagementSystem.DataAccess/DBConnection/DbUtils.cs` — `CheckMerchantCredentialsFromDb`.
- `API/.../CustomerManagementSystem.WebAPI/Program.cs` — `AddJwtBearer` middleware config.
- `API/.../CustomerManagementSystem.WebAPI/Controllers/AuthenticationController.cs`

## How it works

**Issuing** (`POST /api/Authentication/access-token`):
1. `AuthService.GetAccessToken` rejects empty merchant ID/password (`403`), otherwise delegates straight to `JwtCreation.GenerateBearerJwt`.
2. `JwtCreation.GenerateBearerJwt` calls `DbUtils.CheckMerchantCredentialsFromDb`, which fetches the merchant's password hash/salt/role via `usp_getMerchantAuthData` and verifies the password (see [[password-security]]). The success response's `Data` carries the merchant's role (an `int?`) back up.
3. On success, a JWT is minted: symmetric **HMAC-SHA256** signing (`SymmetricSecurityKey` + `HmacSha256Signature`), key from `Auth:SecureJWTKey` in `appsettings.json` (committed value is a demo placeholder — see [[known-gaps]]).
4. **Claims on the token**: `ClaimTypes.Sid` = merchant ID, `JwtRegisteredClaimNames.Sub` = merchant ID, `ClaimTypes.Name` = merchant ID (no separate display name exists yet), `ClaimTypes.Role` = the merchant's role fetched in step 2 (empty string if somehow null), `"amr"` = `"pwd"` (authentication-method-reference, OIDC-style — documents *how* the subject authenticated), `Jti` = random GUID, `Iat` = issue time.
5. Issuer/Audience both `https://localhost:7145/` (demo value), expiry from `Auth:AccessTokenTimeout` (currently `15` minutes).

**Validating** — there is now **one** place tokens are checked, deliberately: `Program.cs`'s `AddJwtBearer` middleware, which runs on every `[Authorize]`-attributed endpoint (all of `CustomerController`, plus `AuthenticationController.VerifyToken`). It checks signature, issuer, audience, and expiry (`ClockSkew = TimeSpan.Zero`, no grace period). `ClaimTypes.Role` is ASP.NET's default role-claim type, so `[Authorize(Roles = "1801")]` would work on any endpoint today without touching the JWT pipeline further, if role-gating is ever needed.

## Gotchas / conventions

- There used to be a **second**, hand-rolled validation path — a `JwtValidation` class used internally by `AuthService` as a self-check right after minting a token, and an unused `HttpClient` built from an `IHttpClientFactory` that was never actually called with. Both were dead code (request-time auth was always enforced solely by the `AddJwtBearer` middleware) and were deleted; `AuthService` now just checks credentials and returns whatever `JwtCreation` produces. If you see references to `JwtValidation.cs` elsewhere (older docs, comments), they're stale.
- The role claim only reflects the merchant's role **at login time** — the token itself isn't re-checked against the DB later, so a role change mid-session only takes effect on the next login (this is inherent to stateless JWTs, not a bug).
- No refresh token, no server-side revocation list — a stolen token is valid until it naturally expires. There is no logout endpoint either; the client just clears its own stored token client-side, see [[angular-app-config-and-routing]].
- `CheckMerchantCredentialsFromDb`'s role-mismatch branch currently echoes the merchant's numeric role back in its `403` message (`"The provided merchant role ({role}) is not valid."`) — a minor info-disclosure smell (requires already-valid credentials to trigger), tracked as an open item, see [[known-gaps]].
