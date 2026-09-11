# Password Security

## What it is

How merchant passwords are hashed, stored, and verified.

## Key files / paths

- `API/.../CustomerManagementSystem.DataAccess/DBConnection/PasswordHasher.cs`
- `DB/Customer_Management_System_DB/Tables/tbl_merchants.sql`
- `DB/Customer_Management_System_DB/Stored_Procedures/usp_getMerchantAuthData.sql`
- `DB/Customer_Management_System_DB/Post_Deployment_Scripts/post_deployment_populate_tbl_merchants.sql`

## How it works

- `PasswordHasher.cs`: **PBKDF2-HMACSHA256**, 100,000 iterations, 16-byte random salt, 32-byte hash (`Rfc2898DeriveBytes.Pbkdf2`). Verification uses `CryptographicOperations.FixedTimeEquals` — a constant-time comparison, not `==`, so timing doesn't leak how many leading bytes matched.
- `tbl_merchants` stores `merchant_password` (BINARY 32, the hash) and `merchant_password_salt` (BINARY 16) as **separate columns** — the plaintext password is never stored or logged anywhere.
- `usp_getMerchantAuthData` fetches the hash+salt+role for a merchant ID and bumps `last_interaction`; the actual comparison (`PasswordHasher.VerifyPassword`) happens in **C#, not SQL**.
- The seeded demo login (`TestMerchantID` / `Merchant123`) has its hash+salt precomputed and hardcoded in the post-deployment script's `BINARY` literals — it doesn't call `PasswordHasher.HashPassword` at deploy time; the comment there says the values were generated to match this exact scheme.

## Gotchas / conventions

- This replaced an earlier **unsalted SHA-256** scheme — if you ever see unsalted-hash code or a single hash column reappear, that's a regression, not an alternate valid approach.
- There's no rate limiting or account lockout on the login endpoint — PBKDF2 slows a single guess but doesn't stop scripted brute-force attempts at network speed. See [[known-gaps]].
