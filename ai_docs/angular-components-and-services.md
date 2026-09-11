# Angular Components & Services

## What it is

The route/component map, the customer services layer, and two real bugs that were fixed here (worth knowing so they aren't reintroduced).

## Key files / paths

- `UI/customer_management_system/src/app/components/*`
- `UI/customer_management_system/src/app/services/*`

## How it works

**Components** (`src/app/components/`):

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

Gender is stored/sent as an **integer**: `0` = Not declared, `1` = Male, `2` = Female — canonicalized across `add-customer` and `edit-customer` forms (both used to disagree on string vs. numeric values; now fixed to match `tbl_customers.gender`).

**Services** (`src/app/services/`):
- `user-login.service.ts`, `verify-token.service.ts`, `auth-guard.service.ts`, `session-storage.service.ts`, `http-header-service.ts`, `auth-error.interceptor.ts` — see [[angular-login-flow]] and [[angular-app-config-and-routing]].
- `get-customer.service.ts`, `add-customer.service.ts`, `edit-customer.service.ts`, `activate-customer.service.ts`, `delete-customer.service.ts` — one per `CustomerController` endpoint group (see [[api-request-pipeline]]).
- `navbar.service.ts`, `footer.service.ts` — simple show/hide state for chrome that shouldn't appear on the login screen.

**Customer lifecycle actions in the UI** (`customer-list.component.ts`, `customer-details.component.ts`): Edit is always available; Deactivate shows only when active; Reactivate and Delete show only when deactivated — mirroring the stored-procedure rules in [[customer-data-model-and-lifecycle]] rather than re-deriving them.

## Gotchas / conventions

- `customer-list.component.ts`'s duplicate-GUID check runs as an `effect()` over the `duplicateGuids` computed signal, in the **constructor** (not `ngOnInit`) — it re-evaluates whenever `customers()` actually changes. A one-time synchronous check right after the async `loadCustomers()` call used to always see a stale/empty list and never fire; don't move this back to a one-shot check in `ngOnInit`.
- See [[angular-app-config-and-routing]] for why `verify-token.service.ts` must stay a simple `pipe(map/catchError)` and not go back to a manually-managed `Subject`.
