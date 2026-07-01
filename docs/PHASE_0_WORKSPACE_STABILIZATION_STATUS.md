# Phase 0 - Workspace Stabilization Status

Date: June 27, 2026.

## Completed

- Created root `AxiCore.sln`.
- Added existing AxiPlus projects to the root solution.
- Added AxiCore shared library skeleton:
  - `AxiCore.SharedKernel`
  - `AxiCore.Contracts`
  - `AxiCore.Diagnostics`
  - `AxiCore.Identity`
  - `AxiCore.Security`
  - `AxiCore.Persistence`
  - `AxiCore.Infrastructure`
- Added shared starter primitives:
  - `Result`
  - `PracticeLaunchRequest`
  - `FunctionTrace`
  - `AxiCoreRoles`
  - `SignedTokenOptions`
  - `DbTransactionRunner`
  - `AxiCoreInfrastructureMarker`
- Updated PostgreSQL Docker configuration from `axiplusdb` to `AxiPlusDb`.
- Added PostgreSQL initialization script for:
  - `AxiCoreDb`
  - `AxiForgeDb`
  - `AxiHireDb`
- Created PostgreSQL databases in the running local `axiplus-postgres` container:
  - `AxiCoreDb`
  - `AxiPlusDb`
  - `AxiForgeDb`
  - `AxiHireDb`
- Updated root README to describe AxiCore instead of AxiForge-only setup.

## Verification

Command:

```powershell
dotnet build .\AxiCore.sln
```

Result:

- Build succeeded.
- 0 warnings.
- 0 errors.

Database verification:

```text
AxiCoreDb
AxiForgeDb
AxiHireDb
AxiPlusDb
axiplusdb
postgres
```

Note:

- `axiplusdb` still exists from the previous AxiPlus setup.
- A stopped `axicore-postgres` container exists from the first Docker attempt because port 5432 was already used by `axiplus-postgres`.

## Next Phase

Phase 1 - AxiPlus Fix & Integration Preparation.

Primary next tasks:

- Run AxiPlus API and Web locally against `AxiPlusDb`.
- Apply migrations to `AxiPlusDb`.
- Add shared diagnostics package references where needed.
- Begin controller/service tracing retrofit.
- Add AxiPlus-to-AxiForge practice launch contracts and endpoint.
- Prepare entitlement checks through AxiCore contracts.
