# Phase 1 - AxiPlus Fix & Integration Preparation Status

Date: June 27, 2026.

## Completed

- Applied AxiPlus EF Core migrations to `AxiPlusDb`.
- Verified AxiPlus API startup against `AxiPlusDb`.
- Seeded demo AxiPlus data into `AxiPlusDb`.
- Added trial student billing entitlement for seeded student access.
- Added AxiCore shared references to AxiPlus projects.
- Added signed AxiPlus-to-AxiForge practice launch contract usage.
- Added `IPracticeLaunchService`.
- Added `PracticeLaunchService`.
- Added `PracticeLaunchController`.
- Added `POST /api/practice-launch/lessons/{lessonId}` endpoint.
- Added `SignedTokens` and `Integrations:AxiForge` configuration.
- Retrofitted the auth login path to the AxiCore tracing standard:
  - `AuthController.Login`
  - `JwtService.GenerateToken`

## Verification

Commands:

```powershell
dotnet ef database update --project .\AxiPlus\AxiPlus.Infrastructure\AxiPlus.Infrastructure.csproj --startup-project .\AxiPlus\AxiPlus.API\AxiPlus.API.csproj
dotnet build .\AxiCore.sln
dotnet run --project .\AxiPlus\AxiPlus.API\AxiPlus.API.csproj --no-build --urls http://localhost:5228
```

Results:

- Migrations applied successfully.
- Build succeeded.
- 0 warnings.
- 0 errors.
- API started successfully on `http://localhost:5228`.
- `StudentBillingAccounts` table exists in `AxiPlusDb`.

## Remaining Phase 1 Work

- Add the AxiPlus Web practice button integration.
- Add UI/service call to `POST /api/practice-launch/lessons/{lessonId}`.
- Continue tracing retrofit across controllers and infrastructure services.
- Add AxiCore entitlement persistence model in `AxiCoreDb`.
- Add tests for practice launch token generation and entitlement denial.
- Clean up old `axiplusdb` only after confirming no needed data remains.
