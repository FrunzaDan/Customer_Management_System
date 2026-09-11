# Known Gaps / Deliberately Deferred

## What it is

Things in this codebase that are known limitations rather than oversights to silently "fix" — read this before assuming something is a bug.

## Key files / paths

- `API/.../CustomerManagementSystem.WebAPI/appsettings.json` — `Auth:SecureJWTKey`
- `API/.../CustomerManagementSystem.DataAccess/DBConnection/DbUtils.cs` — `CheckMerchantCredentialsFromDb`
- `API/.../CustomerManagementSystem.WebAPI/Controllers/AuthenticationController.cs`

## How it works (i.e., what's deferred, and why it's known)

- **`Auth:SecureJWTKey` is a placeholder value** committed in `appsettings.json` — fine for local dev, must be replaced (and pulled out of source control) for any real deployment. Not a live secret to protect; this repo's whole security posture assumes local-only use.
- **No rate limiting or account lockout** on `POST /api/Authentication/access-token`. PBKDF2 (see [[password-security]]) slows a single guess but doesn't stop scripted brute-force/credential-stuffing at network speed.
- **No JWT revocation mechanism.** Tokens carry a `Jti` (see [[jwt-auth-flow]]) but nothing persists or checks it — a stolen token stays valid until it naturally expires. There's also no logout endpoint; the client only clears its own local copy.
- **`CheckMerchantCredentialsFromDb`'s role-mismatch branch leaks the merchant's numeric role** in its `403` message (`"The provided merchant role ({role}) is not valid."`) instead of the generic "Invalid Merchant ID or Password." used for a bad password. Low severity (needs already-valid credentials to trigger) but worth folding into the generic message if this is revisited.
- **Single role in practice**: `1801` is the only `merchant_role` value currently seeded or checked against (`DbUtils.CheckMerchantCredentialsFromDb` hardcodes the `== 1801` check). The JWT does carry a `ClaimTypes.Role` claim now (see [[jwt-auth-flow]]), so per-endpoint `[Authorize(Roles = ...)]` would work if a second role is ever introduced — nothing needs to change in the token pipeline for that, only the DB check and any new `[Authorize]` attributes.

## Gotchas / conventions

- Don't treat this file as a TODO list to clear autonomously — several of these (placeholder JWT key, single role) are appropriate for a local-learning-project scope. The ones worth flagging if asked "is this secure?" are the rate-limiting gap and the role-message leak.
