# Known Gaps / Deliberately Deferred

## What it is

Things in this codebase that are known limitations rather than oversights to silently "fix" — read this before assuming something is a bug.

## Key files / paths

- `API/.../CustomerManagementSystem.WebAPI/appsettings.json` — `Auth:SecureJWTKey`
- `API/.../CustomerManagementSystem.DataAccess/DBConnection/DbUtils.cs` — `CheckMerchantCredentialsFromDb`
- `API/.../CustomerManagementSystem.WebAPI/Controllers/AuthenticationController.cs`
- `API/.../CustomerManagementSystem.BusinessLogic/AuthFunctions/JwtCreation.cs`, `API/.../CustomerManagementSystem.WebAPI/Program.cs` — independent `Encoding.ASCII.GetBytes(key)` calls
- `API/.../CustomerManagementSystem.Domain/Models/ResponseModel.cs`

## How it works (i.e., what's deferred, and why it's known)

- **`Auth:SecureJWTKey` is a placeholder value** committed in `appsettings.json` — fine for local dev, must be replaced (and pulled out of source control) for any real deployment. Not a live secret to protect; this repo's whole security posture assumes local-only use.
- **No rate limiting or account lockout** on `POST /api/Authentication/access-token`. PBKDF2 (see [[password-security]]) slows a single guess but doesn't stop scripted brute-force/credential-stuffing at network speed.
- **No JWT revocation mechanism.** Tokens carry a `Jti` (see [[jwt-auth-flow]]) but nothing persists or checks it — a stolen token stays valid until it naturally expires. There's also no logout endpoint; the client only clears its own local copy.
- **`CheckMerchantCredentialsFromDb`'s role-mismatch branch leaks the merchant's numeric role** in its `403` message (`"The provided merchant role ({role}) is not valid."`) instead of the generic "Invalid Merchant ID or Password." used for a bad password. Low severity (needs already-valid credentials to trigger) but worth folding into the generic message if this is revisited.
- **Single role in practice**: `1801` is the only `merchant_role` value currently seeded or checked against (`DbUtils.CheckMerchantCredentialsFromDb` hardcodes the `== 1801` check). The JWT does carry a `ClaimTypes.Role` claim now (see [[jwt-auth-flow]]), so per-endpoint `[Authorize(Roles = ...)]` would work if a second role is ever introduced — nothing needs to change in the token pipeline for that, only the DB check and any new `[Authorize]` attributes.
- **Most of `IDbUtils` is still `ResponseModel<object>`** (`RegisterCustomer`, `GetCustomer`, `GetCustomers`, `EditCustomer`, `DeactivateCustomer`, `ReactivateCustomer`, `DeleteCustomer`) — deliberately left that way. None of them are ever cast back to a concrete type in C#: mutating ones carry no `Data` at all, and the read ones (`CustomerModel`/`List<object>` of `CustomerModel`) flow straight to JSON serialization in the controller without an intermediate cast. Genericizing all of `IDbUtils` (and `ICustomerService`, every controller signature, every test mock) to close a risk that doesn't actually exist anywhere in these methods would be a large, speculative refactor — see the Resolved section below for the one spot that *did* have a real unsafe cast, which was fixed instead.

## Gotchas / conventions

- Don't treat this file as a TODO list to clear autonomously — several of these (placeholder JWT key, single role, weak `Data` typing) are appropriate for a local-learning-project scope. The ones worth flagging if asked "is this secure?" are the rate-limiting gap and the role-message leak.

## Resolved (kept here for history — don't rediscover these as "new" findings)

- ~~`tbl_merchants` had no primary key/unique constraint, and `merchant_id` was nullable~~ — fixed: `merchant_id` is now `NOT NULL` with `PRIMARY KEY (merchant_id)`.
- ~~`tbl_customers.email`/`msisdn` uniqueness was enforced only in `usp_createCustomer`'s app-level `IF EXISTS` check, racy under concurrent registrations~~ — fixed: `UQ_tbl_customers_email`/`UQ_tbl_customers_msisdn` unique constraints added as a DB-level backstop. The app-level check is still there for the friendly `400` message on the common case; the constraint now closes the concurrent-registration race, degrading to a `500` (via `usp_createCustomer`'s existing `TRY/CATCH`) in the rare case both requests race past the check.
- ~~`SessionStorageService.getSessionAccessToken()` returned sentinel strings (`'ERROR-NO-SESSION-TOKEN'`, `'ERROR-NON-BROWSER-ENVIRONMENT'`) instead of `null` when there was no token, making `HttpHeaderService`'s `if (token)` check always truthy~~ — fixed: it now returns `string | null`, so anonymous requests (including the login POST and SSR calls from Node) no longer carry a bogus `Authorization` header.
- ~~The JWT signing key was derived independently in two places (`JwtCreation.cs` and `Program.cs`, each calling `Encoding.ASCII.GetBytes(key)` separately)~~ — fixed: both now call a single `JwtSigningKey.Create(string)` in `API/.../CustomerManagementSystem.BusinessLogic/AuthFunctions/JwtSigningKey.cs`, so there's structurally one implementation to keep in sync, not two. Behavior is unchanged (still raw ASCII bytes of the configured string, not base64-decoded) — this fixed the duplication risk, not the ASCII-vs-base64 question itself, which is an unrelated, real behavior decision left alone.
- ~~`IDbUtils.CheckMerchantCredentialsFromDb` returned `ResponseModel<object>`, cast back to `int?` via `credentialsCheck.Data as int?` in `JwtCreation.cs`~~ — fixed: the interface, `DbUtils`'s implementation, and the call site now use `ResponseModel<int?>` directly, so the merchant role flows through typed instead of needing a runtime cast. This was scoped narrowly to the one method with an actual cast-back — see the still-open item above for why the rest of `IDbUtils` deliberately wasn't touched.
