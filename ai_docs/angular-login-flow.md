# Angular Login Flow

## What it is

What happens between submitting the login form and landing on `/customers`, including how HTTP failures get turned into user-facing messages.

## Key files / paths

- `UI/customer_management_system/src/app/components/user-login/user-login.component.ts`
- `UI/customer_management_system/src/app/services/user-login.service.ts`
- `UI/customer_management_system/src/app/services/session-storage.service.ts`

## How it works

1. User submits Merchant ID + password → `UserLoginService.login()` → `POST /api/Authentication/access-token`.
2. On success (`200` + a token in the body): the token is stashed via `SessionStorageService` (`sessionStorage`, cleared when the tab closes), the app navigates to `/customers`.
3. On error, the component maps the HTTP status to a message:
   - `403` → "Merchant credentials are incorrect!"
   - `404` → "Endpoint is down!"
   - `0` (no response reached the browser at all — network/TLS-level failure) → "Could not reach the server. It may be offline, or your browser does not trust its security certificate."
   - anything else → `"Server error ({statusCode}). Please try again later."`

## Gotchas / conventions

- `status === 0` is a **deliberately distinct** case: it's what a rejected TLS certificate (e.g. `ERR_CERT_AUTHORITY_INVALID`) looks like to Angular's `HttpClient` — no response body, no real status code — so the message says so instead of defaulting to a generic "server is down," which would be misleading (the server is up; the browser just doesn't trust its cert). See [[local-dev-setup]] for the underlying dev-cert trust issue this message is covering for.
- The token is attached to the login *request itself* too (via `HttpHeaderService`) even though there's nothing to authenticate yet at that point — harmless (an empty/garbage `Authorization` header on an anonymous endpoint), just worth knowing it's not conditional on having a token already.
