# xUnit v3 Testing (on .NET 10 SDK)

## What it is

Non-obvious setup required to run this repo's test project, specific to xUnit **v3** on the .NET 10 SDK — different from a typical xUnit v2 project.

## Key files / paths

- `global.json` (repo root) — `{ "test": { "runner": "Microsoft.Testing.Platform" } }`
- `API/.../CustomerManagementSystem.Tests/CustomerManagementSystem.Tests.csproj`
- `build.sh` — runs `dotnet test` from the repo root.

## How it works

- The test project covers `BusinessLogic` (validations, `JwtCreation` — see [[jwt-auth-flow]], `CustomerRegistration`/`Editing`/`Getting`/`Activation`/`Deletion`) and `DataAccess`'s `PasswordHasher` (see [[password-security]]) — all pure logic, no live DB or Docker needed. This is why `build.sh` runs `dotnet test` *before* the DB/Docker steps.
- Uses `xunit.v3` 4.0.0 (not the older `xunit` v2 meta-package) and `Moq` for mocking `IDbUtils`/`IAppSettingsConfig`.

## Gotchas / conventions

- The **.NET 10 SDK dropped VSTest support for xUnit v3** entirely — `dotnet test` fails with "Testing with VSTest target is no longer supported..." unless the project opts into the new **Microsoft Testing Platform (MTP)** runner. That opt-in is the repo-root `global.json` shown above.
- That `global.json` is discovered by walking **up from the current working directory** `dotnet` is invoked from — not from the project or `.sln` path — so it has to sit somewhere `dotnet test` will actually be run from or below. `build.sh` runs `dotnet test` from the repo root, hence the file living there.
- This is a **separate, unrelated** `global.json` from `DB/Customer_Management_System_DB/global.json`, which only pins the SQL project's SDK version (`8.0.100`) — the two don't conflict, since discovery stops at the nearest one found walking up from wherever the command runs.
- xUnit v3 test projects compile to a **console executable** (`<OutputType>Exe</OutputType>` in the `.csproj`), not a library — the test assembly is its own runner now, a fundamental v3 architecture change from v2.
