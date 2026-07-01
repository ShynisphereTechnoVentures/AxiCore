# Pre-Phase 4A - AxiCore Shared Data Migration Status

## Purpose

Before Phase 4A starts, shared people/access/money data must move toward AxiCore ownership so AxiPlus, AxiForge, and AxiHire do not duplicate user/account data.

## Completed

- Added AxiCore shared identity entities:
  - `AxiCoreUser`
  - `AxiCoreRole`
  - `AxiCoreUserRole`
  - `AxiCoreProductAccess`
- Added normalized product access through AxiCore identity.
- Added `AxiCoreDbContext`.
- Updated AxiForge API to create `AxiCoreDb` schema during local startup.
- Updated AxiForge auth to use `AxiCoreDb.Users` and `AxiCoreDb.UserProductAccess`.
- Removed AxiForge local `AxiForgeAccount` entity.
- Removed AxiForge `Accounts` DbSet and EF relationship ownership from `AxiForgeDbContext`.
- Dropped old `AxiForgeDb."Accounts"` table from local PostgreSQL.
- AxiForge practice activity now keeps `AccountId` as the shared AxiCore user id.

## Not Yet Moved

AxiPlus shared data still exists in AxiPlus because it is deeply connected to current working flows:

- `Users`
- `Roles`
- `Students`
- `StudentBillingAccounts`
- `StudentPayments`
- `SalarySlips`

These must not be deleted from AxiPlus until replacement AxiCore-backed services and migrations are complete.

## Required Next Migration Step

Before removing AxiPlus shared tables, create an AxiCore migration layer for:

- AxiCore users and roles.
- Common student profile.
- Product access for AxiPlus and AxiForge.
- Billing and payments.
- Salary slips or staff payroll.
- Backfill from existing AxiPlus data into AxiCore tables.
- AxiPlus API service changes to read/write shared data through AxiCore services.

## Regression Completed

- Full solution build passed.
- AxiForge direct registration creates an AxiCore user and AxiForge product access.
- AxiForge direct login works through AxiCore identity.
- Old AxiForge `Accounts` table is absent.
- AxiPlus admin login still works.
- AxiForge login page renders.

## Decision

Do not proceed by deleting AxiPlus shared tables until AxiPlus is fully rewired to AxiCore shared identity, billing, and staff/payroll services. AxiForge account duplication has been removed safely.
