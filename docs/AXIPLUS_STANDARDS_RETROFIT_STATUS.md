# AxiPlus Standards Retrofit Status

Date: June 27, 2026.

Latest update: July 3, 2026.

## Goal

Bring AxiPlus into the AxiCore engineering standard before Phase 2 begins.

Required standards:

- Entering trace.
- Exiting trace.
- Exception trace.
- Try/catch behavior.
- Function comments for non-trivial functions.
- Reuse shared helpers.
- Correct database transaction/locking strategy.

## Completed Coverage

### API Controllers

Covered by:

- `FunctionTraceActionFilter`

Coverage:

- Entering action.
- Exiting action.
- Exception logging.
- Applies globally to MVC controller actions.

Manually retrofitted:

- `AuthController.Login`
- `PracticeLaunchController.LaunchLessonPractice`

### Minimal API Endpoints

Covered by:

- `FunctionTraceEndpointFilter`

Coverage:

- Entering endpoint delegate.
- Exiting endpoint delegate.
- Exception logging.
- Applied to:
  - `UserEndpoints`
  - `TrackEndpoints`
  - `StudentEndpoints`
  - `ModuleEndpoints`

### Infrastructure/Application Interface Services

Covered by:

- `TracingProxy<TService>`
- `AddTracedScoped<TInterface, TImplementation>`

Coverage:

- Entering public interface method.
- Exiting public interface method.
- Exception logging.
- Async Task and Task<T> completion-aware tracing.

Applied to:

- `IDashboardService -> DashboardService`
- `IModuleService -> ModuleService`
- `ILessonService -> LessonService`
- `ILessonLiveClassService -> LessonLiveClassService`
- `IAssignmentService -> AssignmentService`
- `IAttendanceService -> AttendanceService`
- `IStudentPortalService -> StudentPortalService`
- `IMentorPortalService -> MentorPortalService`
- `IAdminPortalService -> AdminPortalService`
- `IOperationsService -> OperationsService`
- `IPracticeLaunchService -> PracticeLaunchService`

### Auth and Launch Services

Manually retrofitted:

- `JwtService.GenerateToken`
- `PracticeLaunchService.CreateLessonPracticeLaunchAsync`
- `PracticeLaunchService.CreateSignature`
- `AuthService` public auth functions in Blazor Web

### Blazor Web API Client Services

Covered by:

- `AuthorizedApiClient`

Coverage:

- Shared token-aware request handling.
- Entering trace.
- Exiting trace.
- Exception trace.
- Consistent JSON deserialization and HTTP failure handling.

Retrofitted:

- `AdminPortalApiService`
- `MentorPortalApiService`
- `OperationsApiService`
- `StudentPortalApiService`
- `DashboardApiService`
- `ModuleApiService`
- `LessonApiService`

### Concrete API/Startup Services

Manually retrofitted:

- `BatchAllocationService.AllocateBatchAsync`
- `DataSeeder.SeedAdminAsync`

### Database Access

Current standard applied:

- EF Core scoped DbContext.
- EF Core migration lock verified during migration execution.
- `DbTransactionRunner` added for multi-step write workflows.
- Literal C# `lock` is intentionally not used for normal database access.

## Excluded By Standard

These files are not manually retrofitted unless they gain business behavior:

- DTOs.
- Enums.
- Property-only entities.
- EF Core configuration classes.
- EF Core migrations.
- Generated build artifacts.
- Razor markup-only pages.

## Phase Gate Closure

The AxiPlus standards retrofit items that blocked later phases are complete as of July 3, 2026.

| Item | Result |
| --- | --- |
| MVC action tracing | Complete through `FunctionTraceActionFilter` |
| Minimal API endpoint tracing | Complete through `FunctionTraceEndpointFilter` |
| Interface service tracing | Complete through `TracingProxy<TService>` |
| Auth and practice launch manual tracing | Complete |
| Older Blazor Web API client cleanup | Complete through `AuthorizedApiClient` |
| Database transaction helper standard | Complete |

Future hardening can still expand XML comments and add deeper unit tests, but those are no longer phase blockers for the AxiPlus retrofit.

## Verification

Command:

```powershell
dotnet build .\AxiCore.sln
```

Result:

- Build succeeded.
- 0 warnings.
- 0 errors.
- AxiPlus regression script passed.
- AxiCore shell/API regression script passed.
- Browser-click QA passed for login, dashboard, modules, lessons, practice, notifications, and support.
