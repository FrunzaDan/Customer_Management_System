# Angular App Config & Routing

## What it is

Global HTTP/router wiring, and the route guard that gates every authenticated page.

## Key files / paths

- `UI/customer_management_system/src/environments/environment.ts`
- `UI/customer_management_system/src/app/app.config.ts`
- `UI/customer_management_system/src/app/app.routes.ts`
- `UI/customer_management_system/src/app/services/auth-guard.service.ts`
- `UI/customer_management_system/src/app/services/verify-token.service.ts`
- `UI/customer_management_system/src/app/services/auth-error.interceptor.ts`

## How it works

**Config** (`environment.ts`):
```ts
export const environment = {
  CustomerManagementSystemAPI: 'https://localhost:7145',
  EmailRegex: "^\\S+@\\S+\\.\\S+$",
  PhoneRegex: "^[0-9]{9,12}$",
  UserName: "^[a-zA-z0-9\\ ]*$"
};
```
Every service builds its request URL off `CustomerManagementSystemAPI`.

**`app.config.ts`** wires up: Router (component input binding, view transitions, scroll restoration), **SSR client hydration** (`provideClientHydration(withEventReplay(), withNoIncrementalHydration())`), and `HttpClient` on the **fetch-based backend** (`provideHttpClient(withFetch(), withInterceptorsFromDi(), withInterceptors([authErrorInterceptor]))`) — the fetch-based backend matters for SSR: during server-side rendering, HTTP calls run through Node's native `fetch()`, not a browser's (see [[local-dev-setup]] for the TLS implication of that).

**Routing & the auth guard** (`app.routes.ts`, `auth-guard.service.ts`): all routes except `/login` and the catch-all 404 carry `canActivate: [authGuardFn]`. The guard:
1. Calls `VerifyTokenService.isTokenValid()`, which hits `GET /api/Authentication/verify-token` with whatever token is in session storage.
2. `200` → allow navigation. Anything else (401, network error, TLS error) → clear session storage, redirect to `/login?sessionExpired=true`.

Because routes are guarded and the app uses SSR, this guard's HTTP call can run inside **Node** (server-side) as well as in the browser (client-side, post-hydration).

**Global 401 handling** (`auth-error.interceptor.ts`): a functional `HttpInterceptorFn` (`authErrorInterceptor`) catches any 401 from any API call — except calls to `/api/Authentication/*`, which manage their own 401/403 semantics — clears session storage, and redirects to `/login?sessionExpired=true`. This exists so an expired token discovered mid-session (not just at navigation time, when the guard checks) is handled uniformly, without every component needing its own 401 branch.

## Gotchas / conventions

- `verify-token.service.ts`'s `isTokenValid()` is deliberately a simple `pipe(map(() => true), catchError(() => of(false)))`. An earlier version used a manually-managed RxJS `Subject` that crashed (`Cannot read properties of null`) whenever the API returned a 401 with an empty body, orphaning the guard's subscription and cascading into hydration timeouts and an unclickable login form. **Don't reintroduce a manual `Subject` here.**
- The guard and the interceptor are two separate 401-handling paths by design (navigation-time vs. any-time-during-a-page) — don't collapse them into one without checking both are still exercised (route guards don't run for API calls made without a navigation, e.g. a button click on a page you're already on).
