# Phase 4A - AxiForge Dynamic Data Status

Date: June 29, 2026

## Status

Phase 4A is complete for the AxiForge student-facing surfaces currently in scope.

The AxiForge UI no longer uses hardcoded LeetCode-style catalog data, fake demo credentials, fixed Binary Search study plan content, or Blazor template demo pages. Practice, roadmap, and coding IDE screens now render from API/database DTOs and show neutral empty states when no admin-published records exist.

## Completed Changes

- Converted `AxiForge.Web` practice catalog to render topic cards, topic chips, difficulty filters, and problem rows from `api/coding/problems`.
- Removed hardcoded feature cards, trending skills, fake acceptance percentages, and fixed solved count assumptions from the practice catalog.
- Converted the AxiForge study plan page to render published roadmap templates from `api/roadmaps`.
- Added roadmap detail support in `AxiForgeApiClient.GetRoadmapAsync(Guid roadmapId)` for `api/roadmaps/{roadmapId}`.
- Removed hardcoded Binary Search roadmap rows, related plans, summary bullets, and static roadmap metadata.
- Updated the coding IDE page to use the problem title/topic/difficulty from database-backed problem details.
- Removed hardcoded problem numbering and fixed Binary Search IDE header.
- Removed the AxiForge demo login shortcut and hardcoded demo credentials.
- Kept login, Forge account registration, forgot-password entry point, and returnUrl navigation working from the cleaned login page.
- Removed unused Blazor template `Counter` and `Weather` pages from AxiForge.
- Added empty-state styling for database-empty catalog and roadmap screens.

## Verification

- `dotnet build .\AxiCore.sln --configuration Debug`
- Result: build succeeded with 0 warnings and 0 errors.
- Static scan for removed fake content passed for:
  - `Binary Search`
  - `LeetCode`
  - `Top Interview`
  - `forge.student`
  - `Forge@123`
  - `demo student`
  - `starter problems`
  - `skill tracks`
  - `FeatureCards`
  - `TrendingSkills`

## Smoke Test Note

HTTP smoke startup was attempted for:

- `http://localhost:5055/api/health`
- `http://localhost:5055/api/coding/problems`
- `http://localhost:5055/api/roadmaps`
- `http://localhost:5242/login`
- `http://localhost:5242/practice`
- `http://localhost:5242/roadmaps`

The apps did not start because the local PowerShell `Start-Process` call failed before launch with duplicate `Path/PATH` environment keys. This is a local process-start issue, not a compile issue. Manual Visual Studio / VS Code launch should be used for visual browser validation.

## Phase 4B Dependency

Phase 4A makes the student portal ready for dynamic content, but admin authoring is still required in Phase 4B:

- Problem statement CRUD
- Test case CRUD
- Topic, difficulty, language, class, company, and roadmap mappings
- MCQ authoring
- Publish/unpublish flow
- AxiPlus admin side menu entry for AxiForge content management

## Phase 4C Dependency

Registration and login are working through AxiCore-shared identity for AxiForge. Phase 4C still needs the full cross-product registration process:

- AxiPlus student registration creates AxiCore user once.
- AxiPlus registration grants both AxiPlus and AxiForge access immediately.
- AxiForge direct registration grants AxiForge access only.
- Email confirmation is sent for both product access paths.
