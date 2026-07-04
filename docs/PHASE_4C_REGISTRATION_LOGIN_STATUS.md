# Phase 4C - Registration and Login Status

Date: June 29, 2026

Latest update: July 3, 2026

## Status

Phase 4C account provisioning, email confirmation, resend confirmation, and delivery-provider hardening are implemented for shared AxiCore identity and product access.

AxiPlus student creation now creates or updates a shared AxiCore user and grants both AxiPlus and AxiForge product access immediately. AxiForge direct registration remains AxiForge-only.

## Implemented

- Added AxiCore provisioning into AxiPlus API.
- AxiPlus API now connects to `AxiCoreDb`.
- AxiPlus API ensures the AxiCore schema exists at startup.
- Added `AxiCoreAccountProvisioningService`.
- AxiPlus student self-registration now grants:
  - `AxiPlus`
  - `AxiForge`
- AxiPlus generic student creation now grants:
  - `AxiPlus`
  - `AxiForge`
- AxiPlus Admin Portal student creation now grants:
  - `AxiPlus`
  - `AxiForge`
- AxiForge direct registration continues to grant:
  - `AxiForge` only
- Shared AxiCore users are keyed by normalized email to avoid duplicate cross-product accounts.
- Shared Student role is created/assigned in AxiCore when needed.
- Product access rows are created only when missing.
- AxiForge registration sends an email-confirmation message through the configured delivery provider.
- Password reset sends through the configured delivery provider.
- AxiForge Web includes a confirmation page at `/confirm-email`.
- AxiForge Web login includes a resend-confirmation action.
- AxiForge API exposes confirmation endpoints:
  - `POST /api/auth/confirm-email`
  - `POST /api/auth/resend-confirmation`

## Email Confirmation

The AxiCore user is created with `EmailConfirmed = false`, and AxiForge now issues short-lived signed purpose tokens for email confirmation.

Delivery modes:

- `EmailDelivery:Mode = Console`: local development fallback that writes the confirmation/reset link to logs.
- `EmailDelivery:Mode = Smtp`: sends through the configured SMTP host, port, credentials, sender, and SSL setting.

External delivery is now a deployment configuration task, not a missing application feature.

## Verification

- `dotnet build .\AxiCore.sln --configuration Debug`
- Result: build succeeded with 0 warnings and 0 errors.
- `scripts/Invoke-AxiCoreRegression.ps1`
- Result: pass.
- Regression coverage includes password reset delivery flow, resend confirmation endpoint, and invalid confirmation token rejection.

## Phase 4C Closure

The previous Phase 4C hardening list is complete as of July 3, 2026.

| Closure Item | Result |
| --- | --- |
| Email provider integration | Complete with SMTP plus console fallback |
| Email confirmation token generation | Complete |
| Email confirmation endpoint | Complete |
| Resend confirmation action | Complete |
| Regression coverage for auth hardening | Complete |

The later migration to move every remaining AxiPlus local auth read fully onto AxiCore identity is a broader identity-consolidation project, not a Phase 4C blocker.
