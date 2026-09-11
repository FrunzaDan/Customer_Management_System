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
| `customer-list` | (used by `home`) | Table of all customers, `@for` track-by GUID; client-side search + pagination |
| `customer-details` | `/customerDetails` | Single customer's full record, with friendly (not raw-numeric) status |
| `add-customer` | `/addCustomer` | Create-customer form |
| `edit-customer` | `/editCustomer` | Edit-customer form |
| `features` | `/features` | Static feature list page |
| `about` | `/about` | Static about page |
| `navigation-bar` | (shown/hidden via `NavbarService`) | Top nav — hidden on `/login`; owns the "Log out" action |
| `footer` | (shown/hidden via `FooterService`) | Page footer — hidden on `/login` |
| `display-error` | n/a | Reusable error display |
| `page-not-found` | `**` | 404 fallback |

Gender is stored/sent as an **integer**: `0` = Not declared, `1` = Male, `2` = Female — canonicalized across `add-customer` and `edit-customer` forms (both used to disagree on string vs. numeric values; now fixed to match `tbl_customers.gender`).

**Services** (`src/app/services/`):
- `user-login.service.ts`, `verify-token.service.ts`, `auth-guard.service.ts`, `session-storage.service.ts`, `http-header-service.ts`, `auth-error.interceptor.ts` — see [[angular-login-flow]] and [[angular-app-config-and-routing]].
- `get-customer.service.ts`, `add-customer.service.ts`, `edit-customer.service.ts`, `activate-customer.service.ts`, `delete-customer.service.ts` — one per `CustomerController` endpoint group (see [[api-request-pipeline]]).
- `navbar.service.ts`, `footer.service.ts` — simple show/hide state for chrome that shouldn't appear on the login screen.

**Customer lifecycle actions in the UI** (`customer-list.component.ts`, `customer-details.component.ts`): Edit is always available; Deactivate shows only when active; Reactivate and Delete show only when deactivated — mirroring the stored-procedure rules in [[customer-data-model-and-lifecycle]] rather than re-deriving them. Both Deactivate and Delete are guarded by a native `confirm()` prompt before the request fires; Reactivate and Edit are not (both are non-destructive/reversible).

**Search & pagination** (`customer-list.component.ts`): both are client-side, over the already-loaded `customersSignal` — there's no server-side "list customers with a filter/page" endpoint (`GetCustomers` always returns everything; only single-customer lookup by exact GUID/MSISDN/email exists server-side, used by the details page, not by list search). `searchTerm` → `filteredCustomers` (substring match across first/last name, email, MSISDN) → `pagedCustomers` (sliced by `currentPage`/`pageSize`, 10/page). Typing a new search term resets to page 1. Pagination controls only render when there's more than one page — with the two seeded demo customers you'll never see them locally; that's expected, not a bug.

**Birthdate is a native `<input type="date">`** in both `add-customer` and `edit-customer` — replaced three separate year/month/day text boxes. The API still just wants `YYYY-MM-DD` (that's what `usp_createCustomer`/`usp_editCustomer` store as-is in `tbl_customers.birthdate`, an `NVARCHAR`, not a real `DATE` column), and a date input's `.value` is always exactly that format, so no manual concatenation is needed on submit anymore. When *editing* an existing customer, `EditCustomerComponent.toDateInputValue()` zero-pads the stored value before patching the form — a date input silently fails to pre-fill on anything not strictly zero-padded, and the old three-box form could have saved e.g. `"2020-1-5"` for some existing records.

## Gotchas / conventions

- `customer-list.component.ts`'s duplicate-GUID check runs as an `effect()` over the `duplicateGuids` computed signal, in the **constructor** (not `ngOnInit`) — it re-evaluates whenever `customers()` actually changes. A one-time synchronous check right after the async `loadCustomers()` call used to always see a stale/empty list and never fire; don't move this back to a one-shot check in `ngOnInit`.
- See [[angular-app-config-and-routing]] for why `verify-token.service.ts` must stay a simple `pipe(map/catchError)` and not go back to a manually-managed `Subject`.
- `NavigationBarComponent.logout()` explicitly calls `SessionStorageService.removeSessionStorage()` before navigating to `/login` — don't replace it with a plain `routerLink="login"` again. It used to be exactly that, and only "worked" because `UserLoginComponent.ngOnInit()` happens to clear storage too; that's an implicit dependency on another component's unrelated side effect, not something logout should rely on. See [[known-gaps]] for the fix history.
