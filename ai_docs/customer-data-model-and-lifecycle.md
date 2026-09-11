# Customer Data Model & Lifecycle

## What it is

The `tbl_customers`/`tbl_addresses` schema, the status codes that drive the deactivate/reactivate/delete lifecycle, and how a customer is looked up.

## Key files / paths

- `DB/Customer_Management_System_DB/Tables/tbl_customers.sql`, `tbl_addresses.sql`, `tbl_merchants.sql`
- `DB/Customer_Management_System_DB/Stored_Procedures/usp_createCustomer.sql`, `usp_getCustomer.sql`, `usp_getCustomers.sql`, `usp_editCustomer.sql`, `usp_deactivateCustomer.sql`, `usp_reactivateCustomer.sql`, `usp_deleteCustomer.sql`
- `API/.../CustomerManagementSystem.BusinessLogic/CustomerFunctions/*.cs` — `CustomerRegistration`, `CustomerEditing`, `CustomerGetting`, `CustomerActivation`, `CustomerDeletion`

## How it works

**`tbl_customers`**: `PK_customer_guid` (NVARCHAR 50, app-generated), `first_name`, `last_name`, `email`, `msisdn`, `gender` (INT — see [[angular-components-and-services]] for the 0/1/2 mapping), `birthdate`, `customer_Status`, `creation_Date`, `interaction_Date`.

**`tbl_addresses`**: 1:1 with a customer via `FK_customer_guid` (`ON UPDATE CASCADE`) — country/county/town/zip/street/number. `usp_createCustomer` inserts both rows in **one transaction** — `usp_getCustomer`/`usp_getCustomers` `INNER JOIN` the two tables, so a customer row without a matching address row would silently disappear from every read despite existing in `tbl_customers`.

**Status codes** (`tbl_customers.customer_Status`):
- `1901` = active
- `1903` = deactivated

**Lifecycle rules, enforced in the stored procedures themselves** (not just the API layer):
- New customers are created **active** (`usp_createCustomer`).
- `usp_deactivateCustomer` only updates rows where status `<> 1903` → `409` if already deactivated.
- `usp_reactivateCustomer` only updates rows where status `<> 1901` → `409` if already active.
- **`usp_deleteCustomer` requires status `= 1903`** — a customer must be deactivated first; deleting an active customer returns `409` with `'Customer must be deactivated before it can be deleted.'`. Only `deactivate → delete` or `deactivate → reactivate` are valid paths; `deactivate → delete → reactivate` is not (the customer no longer exists after delete).
- `usp_createCustomer` also rejects duplicate `email` or `msisdn` up front with a `400`, before attempting the insert.

**Lookup**: `CustomerGetting.GetCustomerFunction` doesn't take an explicit search type from the caller — it auto-detects one via `DetermineSearchOption`, trying (in order) GUID format → MSISDN format → email format, and passing the matching `SearchOption` (1/2/3) to `usp_getCustomer`'s `@var_SearchOption`. An unrecognized `searchVariable` shape returns `404` without ever hitting the DB.

**GUIDs are always server-generated**: `CustomerRegistration.RegisterCustomerFunction` overwrites `request.Guid` with a fresh `Guid.NewGuid()` before calling the DB — a client can never choose its own customer ID.

## Gotchas / conventions

- `usp_editCustomer` uses `ISNULL(@param, column)` for every field, so **omitting a field in the edit request leaves it unchanged** rather than nulling it out — this is a partial-update endpoint, not a full replace.
- Every mutating stored proc's `(result, message)` convention is described in [[api-request-pipeline]] — `result = 0` success, nonzero maps directly to the HTTP status returned to the client.
