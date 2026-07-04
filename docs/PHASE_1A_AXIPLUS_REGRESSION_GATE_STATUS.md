# Phase 1A - AxiPlus Data Cleanup, Flow Validation & Regression Gate Status

Date: June 28, 2026

Latest update: July 3, 2026

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
- Completed browser-click QA through the Blazor UI for login, dashboard, modules, module detail, lesson detail, practice, notifications, and support.
- Completed older AxiPlus Web API client cleanup through the shared `AuthorizedApiClient` service.

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
| `http://localhost:5094/login` | Pass |
| `http://localhost:5094/student/dashboard` | Pass |

### Automated Regression Script

Command:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Invoke-AxiPlusRegression.ps1 -ApiBaseUrl http://localhost:5228
```

Result:

- Pass

Latest rerun:

- `dotnet build .\AxiCore.sln`
- `scripts/Invoke-AxiCoreRegression.ps1`
- `scripts/Invoke-AxiPlusRegression.ps1`

Result:

- Pass.
- The missing demo-account regression blocker was fixed by restoring deterministic `Demo` mode seeding at AxiPlus API startup.
- Verified logins for SuperAdmin, Admin, MainMentor, AssistantMentor, Student, and CollegeCoordinator.
- Verified student dashboard, profile, modules, billing, lesson, portal, and practice-launch API flows.

Latest browser-click QA:

- Logged in through AxiPlus Web as `child@axiplus.com`.
- Verified student dashboard after login.
- Clicked through `My Modules`, `Open Module`, lesson `Open`, `Practice`, `Notifications`, and `Support`.
- Confirmed expected headings, links, action buttons, and support form fields on each destination.

## Current Local URLs

- AxiPlus API: `http://localhost:5228`
- AxiPlus Web: `http://localhost:5094`

## Phase 1A Closure

The previous Phase 1A blockers are complete as of July 3, 2026.

| Closure Item | Result |
| --- | --- |
| Browser-click login QA | Pass |
| Browser-click dashboard QA | Pass |
| Browser-click modules, lessons, practice, notifications, support QA | Pass |
| Older AxiPlus Web API client cleanup | Complete |
| Automated AxiPlus regression gate | Pass |

## Recommendation

Phase 1A is closed. The deterministic seed, API flows, browser-click UI path, web smoke checks, standards cleanup, and practice launch flow are green enough for continued product development.
