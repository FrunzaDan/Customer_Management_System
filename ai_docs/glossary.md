# Glossary

Domain terms and magic numbers used across this codebase — check here before assuming a number or acronym is arbitrary.

- **Merchant** — the API's authenticated principal; the user who logs in and manages customers. Stored in `tbl_merchants`. Not the same as a "customer."
- **Customer** — the record being managed (name, contact info, address). Stored in `tbl_customers` + `tbl_addresses`.
- **`customer_Status` codes** — `1901` = active, `1903` = deactivated. See [[customer-data-model-and-lifecycle]].
- **`merchant_role` codes** — `1801` = the only role currently in use. See [[known-gaps]].
- **GUID** — customer primary key, always server-generated (`Guid.NewGuid()`), never client-supplied. See [[customer-data-model-and-lifecycle]].
- **ADO.NET** — .NET's low-level data access API (`SqlConnection`/`SqlCommand`/`SqlDataReader`); this project uses it directly against stored procedures, with no ORM (no Entity Framework) in between.
- **Stored-proc result convention** — every mutating stored procedure returns a `(result INT, message NVARCHAR)` row: `result = 0` means success, any nonzero value is the HTTP status the API should return. See [[api-request-pipeline]].
- **`.sqlproj` / `.dacpac`** — the DB schema is an SSDT SQL Server Database Project (`.sqlproj`), which builds to a `.dacpac` (a schema snapshot) that `sqlpackage` diffs against the live database and publishes. Not migration scripts.
- **PBKDF2** — Password-Based Key Derivation Function 2; the password-hashing algorithm used here (SHA-256, 100k iterations, per-user salt). See [[password-security]].
- **JWT / Bearer token** — the API issues a signed JSON Web Token on login; the client sends it back as `Authorization: Bearer <token>` on every subsequent request. Stateless — no server-side session store. See [[jwt-auth-flow]].
- **SSR / hydration** — the Angular app renders server-side first (via `@angular/ssr`, an Express server), then "hydrates" in the browser to become interactive. Relevant because SSR's HTTP calls run through Node, not the browser — see [[local-dev-setup]].
- **MTP (Microsoft Testing Platform)** — the newer .NET test-running infrastructure that xUnit v3 requires on the .NET 10 SDK, in place of the older VSTest pipeline. See [[xunit-v3-testing]].
- **`sessionStorage`** — where the Angular app keeps the JWT client-side; cleared automatically when the browser tab closes (as opposed to `localStorage`, which would persist across sessions).
