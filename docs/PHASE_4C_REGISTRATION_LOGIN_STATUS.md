# Phase 4C - Registration and Login Status

Date: June 29, 2026

## Status

Phase 4C account provisioning is implemented for shared AxiCore identity and product access.

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

## Email Confirmation

The AxiCore user is created with `EmailConfirmed = false`, and the provisioning flow logs that email confirmation is pending for both product access paths.

Actual email sending still requires an SMTP/provider integration. Until that provider is configured, the system cannot truly deliver confirmation email externally.

## Verification

- `dotnet build .\AxiCore.sln --configuration Debug`
- Result: build succeeded with 0 warnings and 0 errors.

## Remaining Hardening

- Add real email provider integration.
- Add email confirmation token generation and confirmation endpoint.
- Add resend confirmation action.
- Move remaining local AxiPlus auth reads fully onto AxiCore identity in a later migration.
- Add integration tests for:
  - AxiPlus registration grants AxiPlus + AxiForge.
  - AxiForge registration grants only AxiForge.
  - Existing AxiCore user receives missing product access without duplication.
