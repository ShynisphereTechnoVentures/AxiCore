# AxiCore

AxiCore is the shared Axionora engineering workspace for:

- **AxiPlus** - mentor-led LMS and training platform.
- **AxiForge** - coding practice, assessments, AI interviews, and career readiness.
- **AxiHire** - recruiter verification and hiring workflows.

## Current Phase

AxiHire foundation has started after the AxiPlus/AxiForge automated regression gate was restored.

Current status:

- Existing AxiPlus solution is included.
- Root `AxiCore.sln` is created.
- Shared library skeleton is created under `AxiCore.Shared`.
- AxiPlus and AxiForge automated regression checks are green.
- AxiHire foundation projects, API, database context, and web shell are included.
- PostgreSQL uses four databases: `AxiCoreDb`, `AxiPlusDb`, `AxiForgeDb`, and `AxiHireDb`.

## Build

```powershell
dotnet build .\AxiCore.sln
```

## Local Database

The current Docker Compose file is under `AxiPlus/docker-compose.yml`.

It starts PostgreSQL and initializes:

- `AxiPlusDb`
- `AxiCoreDb`
- `AxiForgeDb`
- `AxiHireDb`

## Workspace Structure

```text
AxiCore.Shared/
  AxiCore.SharedKernel/
  AxiCore.Contracts/
  AxiCore.Identity/
  AxiCore.Diagnostics/
  AxiCore.Security/
  AxiCore.Persistence/
  AxiCore.Infrastructure/
AxiPlus/
AxiForge/
AxiHire/
docs/
docker/
```

## Planning Documents

- [Master Development Document](docs/MASTER_DEVELOPMENT_DOCUMENT.md)
- [Software Architecture Document](docs/SOFTWARE_ARCHITECTURE_DOCUMENT.md)
- [AxiCore Product Release Roadmap](docs/AXICORE_PRODUCT_RELEASE_ROADMAP.md)
- [AxiCore Engineering Standards](docs/AXICORE_ENGINEERING_STANDARDS.md)
- [Phase 0 Workspace Stabilization Status](docs/PHASE_0_WORKSPACE_STABILIZATION_STATUS.md)
- [Phase 1 AxiPlus Integration Status](docs/PHASE_1_AXIPLUS_INTEGRATION_STATUS.md)
- [Phase 1A AxiPlus Regression Gate Status](docs/PHASE_1A_AXIPLUS_REGRESSION_GATE_STATUS.md)
- [Phase 2 AxiForge Foundation Status](docs/PHASE_2_AXIFORGE_FOUNDATION_STATUS.md)
- [Phase 3 AxiForge Coding Practice Status](docs/PHASE_3_AXIFORGE_CODING_PRACTICE_STATUS.md)
- [Phase 4 AxiForge Roadmaps Assessments RC Status](docs/PHASE_4_AXIFORGE_ROADMAPS_ASSESSMENTS_RC_STATUS.md)
- [Phase 4A/4B/4C AxiForge Dynamic Admin Auth Plan](docs/PHASE_4A_4B_4C_AXIFORGE_DYNAMIC_ADMIN_AUTH_PLAN.md)
- [Pre-Phase 4A AxiCore Shared Data Migration Status](docs/PRE_PHASE_4A_AXICORE_SHARED_DATA_MIGRATION_STATUS.md)
- [AxiPlus Standards Retrofit Status](docs/AXIPLUS_STANDARDS_RETROFIT_STATUS.md)
- [Phase 8 AxiHire Foundation Status](docs/PHASE_8_AXIHIRE_FOUNDATION_STATUS.md)
