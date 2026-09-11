# API Validation

## What it is

Where request validation happens in the API before a stored procedure is ever called.

## Key files / paths

- `API/.../CustomerManagementSystem.BusinessLogic/CustomerFunctions/CustomerRegistration.cs`
- `API/.../CustomerManagementSystem.BusinessLogic/CustomerFunctions/CustomerEditing.cs`
- `API/.../CustomerManagementSystem.BusinessLogic/Validations/GUIDValidation.cs`, `EmailValidation.cs`, `MSISDNValidation.cs`
- `API/.../CustomerManagementSystem.BusinessLogic/Constants/RegexConstants.cs`

## How it works

- **`CustomerRegistration`**: rejects missing/invalid `Email` or `Msisdn` (`400`), rejects a missing `Address` (`400` — `usp_createCustomer`'s address parameters have no SQL-side defaults, so a missing address would otherwise surface as an opaque `500` instead of a clean validation error), then always overwrites the GUID server-side (see [[customer-data-model-and-lifecycle]]).
- **`CustomerEditing`**: GUID is required and format-validated via `GuidValidation.ValidateGuid` (regex-based, `[GeneratedRegex]`); `Email`/`Msisdn`, if present in the request, are format-validated before the edit is attempted — but unlike registration, they're optional here (partial update).
- Email/MSISDN **uniqueness** is enforced in `usp_createCustomer` itself (`400` if either already exists) — this is a DB-layer check, not duplicated in C#.
- The same regexes exist client-side too, in Angular's `environment.ts` (`EmailRegex`, `PhoneRegex`, `UserName`) — kept in sync **manually**, not shared/generated from one source.

## Gotchas / conventions

- Validation failures return `400` with a plain message, distinct from the `404`s used for "not found"/"no valid search key" and `409`s used for lifecycle conflicts (see [[customer-data-model-and-lifecycle]]) — status code choice is meaningful here, not arbitrary.
- If you change a regex, remember there are two copies (C# `Validations/`, Angular `environment.ts`) — nothing enforces they match.
