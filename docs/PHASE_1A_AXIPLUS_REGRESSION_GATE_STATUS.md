# Phase 1A - AxiPlus Data Cleanup, Flow Validation & Regression Gate Status

Date: June 28, 2026

## Decision

Phase 1A should be completed before Phase 2 starts. The AxiPlus login/dashboard issue showed that AxiForge must not be built on top of unstable AxiPlus auth, dashboard, seed, or practice-launch behavior.

## Completed

- Replaced the monolithic startup seed with controlled seed modes.
- Added startup database migration before controlled seeding.
- Added `SeedData:Mode` configuration.
- Cleaned and recreated only `AxiPlusDb`.
- Rebuilt `AxiPlusDb` from EF Core migrations.
- Seeded deterministic local regression data through controlled `Demo` mode.
- Verified all six demo role logins.
- Verified student dashboard, profile, modules, billing, lesson, portal, and practice-launch API flows.
- Verified AxiPlus Web login and student dashboard pages return successfully.
- Added repeatable API regression script: `scripts/Invoke-AxiPlusRegression.ps1`.

## Seed Modes

| Mode | Purpose |
| --- | --- |
| `None` | Disable startup seeding. |
| `Required` | Seed only required platform reference data, currently roles. |
| `Demo` | Seed required data plus deterministic demo accounts, learning catalog, student, billing, attendance, assignments, live classes, notifications, and support ticket data. |

Current local mode:

```json
"SeedData": {
  "Mode": "Demo"
}
```

## Clean Database Validation

Database reset scope:

- Reset: `AxiPlusDb`
- Not touched: `AxiCoreDb`, `AxiForgeDb`, `AxiHireDb`

Validation:

- API startup completed after database recreation.
- EF Core migrations rebuilt the schema.
- Controlled demo seed populated the regression flow data.

## Regression Results

### Authentication

| Account | Expected Role | Result |
| --- | --- | --- |
| `sa@axiplus.com` | SuperAdmin | Pass |
| `admin@axiplus.com` | Admin | Pass |
| `mm@axiplus.com` | MainMentor | Pass |
| `am@axiplus.com` | AssistantMentor | Pass |
| `child@axiplus.com` | Student | Pass |
| `col@axiplus.com` | CollegeCoordinator | Pass |

### Student API Flow

| Flow | Result |
| --- | --- |
| Student dashboard | Pass |
| Student profile | Pass |
| Student modules | Pass |
| Student payments | Pass |
| Student plans | Pass |
| Upcoming payment | Pass |
| Student portal live classes | Pass |
| Student portal recordings | Pass |
| Student portal practice | Pass |
| Student portal notifications | Pass |
| Student portal support tickets | Pass |
| Lessons by module | Pass |
| Lesson details | Pass |
| AxiPlus-to-AxiForge practice launch | Pass |

### Web Smoke Test

| Page | Result |
| --- | --- |
| `http://localhost:5092/login` | Pass |
| `http://localhost:5092/student/dashboard` | Pass |

### Automated Regression Script

Command:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Invoke-AxiPlusRegression.ps1 -ApiBaseUrl http://localhost:5228
```

Result:

- Pass

## Current Local URLs

- AxiPlus API: `http://localhost:5228`
- AxiPlus Web: `http://localhost:5092`

## Remaining Before Phase 1A Final Completion

- Browser-click test login through Blazor UI.
- Browser-click test student dashboard after login.
- Browser-click test module, lesson, practice, notifications, and support pages.
- Finish standards retrofit for older AxiPlus Web service methods that still contain ad hoc console logging.

Browser-click testing note:

- Attempted from Codex, but the local in-app browser tooling failed before connecting to the app.
- Manual browser verification is still required before marking Phase 1A fully complete.

## Recommendation

Phase 2 can start after the browser-click regression checklist passes, because the clean database, controlled seed, API flows, and web page smoke tests are now stable.
