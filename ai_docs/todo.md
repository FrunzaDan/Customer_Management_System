# Concept Doc Status

Tracks what's been documented in `ai_docs/` vs. what's still pending, per the process in `learning_approach.md`.

## Documented

- [[project-overview]]
- [[local-dev-setup]]
- [[api-request-pipeline]]
- [[customer-data-model-and-lifecycle]]
- [[api-validation]]
- [[xunit-v3-testing]]
- [[jwt-auth-flow]]
- [[password-security]]
- [[angular-app-config-and-routing]]
- [[angular-login-flow]]
- [[angular-components-and-services]]
- [[known-gaps]]

All of the above were written in one pass by reading the actual current source (not carried over from an older doc without verification) — see each file's content for the specific files read.

## Pending

Nothing queued right now. Add an entry here when the user starts teaching a new concept, then move it to Documented once the corresponding `.md` file exists and is linked from `index.md`.

## Notes for next session

- The JWT claims and validation path changed mid-session when these docs were written (a dead `JwtValidation.cs` self-check was deleted, and new claims — `sub`, `name`, `role`, `amr` — were added to the token). [[jwt-auth-flow]] reflects the *current* state; don't trust older descriptions elsewhere (e.g. `Documentation/` PDFs/diagrams) that predate this.
- If asked to "harden" the app, [[known-gaps]] already lists the known real gaps (rate limiting, revocation, role-message leak) — check there before re-deriving them from scratch.
