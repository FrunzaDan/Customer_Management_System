# Customer Data Model & Lifecycle

## What it is

The `tbl_customers`/`tbl_addresses` schema, the status codes that drive the deactivate/reactivate/delete lifecycle, and how a customer is looked up.

## Key files / paths

- `DB/Customer_Management_System_DB/Tables/tbl_customers.sql`, `tbl_addresses.sql`, `tbl_merchants.sql`, `tbl_customer_audit_log.sql`
- `DB/Customer_Management_System_DB/Stored_Procedures/usp_createCustomer.sql`, `usp_getCustomer.sql`, `usp_getCustomers.sql`, `usp_editCustomer.sql`, `usp_deactivateCustomer.sql`, `usp_reactivateCustomer.sql`, `usp_deleteCustomer.sql`, `usp_insertCustomerAuditLog.sql`, `usp_getCustomerAuditLog.sql`
- `API/.../CustomerManagementSystem.BusinessLogic/CustomerFunctions/*.cs` — `CustomerRegistration`, `CustomerEditing`, `CustomerGetting`, `CustomerActivation`, `CustomerDeletion`, `CustomerAuditLogger`

## How it works

**`tbl_customers`**: `PK_customer_guid` (NVARCHAR 50, app-generated), `first_name`, `last_name`, `email` (unique), `msisdn` (unique), `gender` (INT — see [[angular-components-and-services]] for the 0/1/2 mapping), `birthdate`, `customer_Status`, `creation_Date`, `interaction_Date`.

**`tbl_addresses`**: 1:1 with a customer via `FK_customer_guid` (`ON UPDATE CASCADE`) — country/county/town/zip/street/number. `usp_createCustomer` inserts both rows in **one transaction** — `usp_getCustomer`/`usp_getCustomers` `INNER JOIN` the two tables, so a customer row without a matching address row would silently disappear from every read despite existing in `tbl_customers`.

**`usp_getCustomers` is paginated**, not a full-table dump: it takes `@PageNumber`, `@PageSize`, an optional `@SearchTerm` (`LIKE '%...%'` against first/last name, email, MSISDN), and `@SortColumn`/`@SortDirection` (`name`/`email`/`msisdn`, `asc`/`desc`). Sorting is done via a parameterized `CASE`-based `ORDER BY` (no dynamic SQL — see the proc for why exactly one `CASE` pair is non-NULL per call) over `OFFSET`/`FETCH`, and each returned row carries a `total_count` column from `COUNT(*) OVER()` so the API can report `TotalItems` without a second query. `CustomerGetting.GetCustomersFunction` validates `PageNumber >= 1`, `1 <= PageSize <= 100`, and that `SortColumn`/`SortDirection` are in their allow-lists — `400` otherwise. The API wraps the page in `PagedResponse<CustomerModel>` (`Domain/Models/PagedResponse.cs`): `PageNumber`, `PageSize`, `TotalItems`, `Items`.

**Status codes** (`tbl_customers.customer_Status`):
- `1901` = active
- `1903` = deactivated

**Lifecycle rules, enforced in the stored procedures themselves** (not just the API layer):
- New customers are created **active** (`usp_createCustomer`).
- `usp_deactivateCustomer` only updates rows where status `<> 1903` → `409` if already deactivated.
- `usp_reactivateCustomer` only updates rows where status `<> 1901` → `409` if already active.
- **`usp_deleteCustomer` requires status `= 1903`** — a customer must be deactivated first; deleting an active customer returns `409` with `'Customer must be deactivated before it can be deleted.'`. Only `deactivate → delete` or `deactivate → reactivate` are valid paths; `deactivate → delete → reactivate` is not (the customer no longer exists after delete).
- `usp_createCustomer` also rejects duplicate `email` or `msisdn` up front with a friendly `400`, before attempting the insert. `UQ_tbl_customers_email`/`UQ_tbl_customers_msisdn` unique constraints on the table back this up as a DB-level safety net for the race window between that check and the insert (two concurrent registrations with the same email/MSISDN); a request that loses that race gets a `500` from the constraint violation instead of the friendlier `400`, via the proc's existing `TRY/CATCH`.

**CSV export** (`GET /api/Customer/export`): `CustomerGetting.GetCustomersForExportFunction` reuses the exact same `_dbUtils.GetCustomers` call (and therefore `usp_getCustomers`) as the paginated `/all` endpoint — it validates/normalizes `SearchTerm`/`SortColumn`/`SortDirection` the same way, but fixes `PageNumber = 1` and `PageSize = MaxExportRows` (5000) internally instead of taking paging from the caller, so export honors the current search/sort but isn't limited to one page. The filtering/sorting SQL lives in exactly one place (`usp_getCustomers`); no separate export stored proc exists. `CustomerCsvExporter.ToCsv` turns the resulting `List<CustomerModel>` into RFC 4180 CSV (with a leading UTF-8 BOM for Excel, and a leading `'` on any field starting with `=`/`+`/`-`/`@` to defang spreadsheet formula injection), and the controller returns it as a `text/csv` `File` result rather than the usual JSON envelope.

**Lookup**: `CustomerGetting.GetCustomerFunction` doesn't take an explicit search type from the caller — it auto-detects one via `DetermineSearchOption`, trying (in order) GUID format → MSISDN format → email format, and passing the matching `SearchOption` (1/2/3) to `usp_getCustomer`'s `@var_SearchOption`. An unrecognized `searchVariable` shape returns `404` without ever hitting the DB.

**GUIDs are always server-generated**: `CustomerRegistration.RegisterCustomerFunction` overwrites `request.Guid` with a fresh `Guid.NewGuid()` before calling the DB — a client can never choose its own customer ID.

**Audit trail** (`tbl_customer_audit_log`): every successful mutation (`Created`/`Edited`/`Deactivated`/`Reactivated`/`Deleted`) writes a row via `CustomerAuditLogger.Log`, called from `CustomerRegistration`/`CustomerEditing`/`CustomerActivation`/`CustomerDeletion` only after the underlying DB call returns `Status == 200`. The merchant ID comes from `CustomerController.MerchantId` (`User.Identity.Name`, set from the JWT's `ClaimTypes.Name` claim — see [[jwt-auth-flow]]), not from the request body. **Deliberately no FK** from `tbl_customer_audit_log.customer_guid` to `tbl_customers` — a deleted customer's audit history must survive `usp_deleteCustomer`, which is the one place this table outlives the row it's about. Logging is **best-effort**: `CustomerAuditLogger.Log` swallows and logs (via `ILogger`) any exception rather than letting it bubble, because it always runs after the customer mutation it's recording has already succeeded — a logging failure must never turn that into a `500`. Read via `GET /api/Customer/auditLog?customerGuid=...` → `usp_getCustomerAuditLog`, newest first; `CustomerGetting.GetCustomerAuditLogFunction` validates the GUID format (`400` if not GUID-shaped) before hitting the DB.

**Global audit log** (`GET /api/Customer/auditLog/all`): a separate, paginated view across every customer, for the `/auditLog` admin page — not the same stored proc as the per-customer trail (that one returns every row for one GUID, unpaginated, which is fine at that scope; a global scan needs real paging). `usp_getAllCustomerAuditLog` follows `usp_getCustomers`'s pagination convention (`@PageNumber`/`@PageSize`, `OFFSET`/`FETCH`, `COUNT(*) OVER()` for `total_count`) but has no search/sort — just newest-first. It `LEFT JOIN`s `tbl_customers` to show a customer name; since the audit table has no FK (see above), a deleted customer's rows come back with `first_name`/`last_name` `NULL` rather than disappearing, and `GlobalAuditLogEntry.CustomerFirstName`/`CustomerLastName` are mapped via `as string` (not `.ToString()`) specifically so that `NULL` surfaces as a real `null`, not `DBNull.Value.ToString()`'s empty string. A dedicated index, `IX_tbl_customer_audit_log_action_Date`, backs this proc's unfiltered `ORDER BY action_Date DESC` scan — the existing `customer_guid` index only helps once a GUID is already known. `CustomerGetting.GetAllAuditLogFunction` validates paging the same way `GetCustomersFunction` does (reusing `MaxPageSize`).

## Gotchas / conventions

- `usp_editCustomer` uses `ISNULL(@param, column)` for every field, so **omitting a field in the edit request leaves it unchanged** rather than nulling it out — this is a partial-update endpoint, not a full replace.
- Every mutating stored proc's `(result, message)` convention is described in [[api-request-pipeline]] — `result = 0` success, nonzero maps directly to the HTTP status returned to the client.
