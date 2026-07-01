# Phase 3 - AxiForge Coding Practice + Judge0 Status

Date: June 28, 2026

## Status

Phase 3 coding practice foundation plus the coding IDE upgrade is implemented and smoke-tested.

## Completed

- Added coding practice domain models:
  - `CodingProblem`
  - `CodingTestCase`
  - `CodingSubmission`
  - `LessonPracticeSet`
- Added coding practice DTOs.
- Added coding practice service contracts.
- Added AxiForge EF Core sets and relationships.
- Added starter problem bank seed data.
- Added local Judge0-compatible execution adapter.
- Added coding practice application service.
- Added coding API endpoints:
  - `GET /api/coding/problems`
  - `GET /api/coding/problems/{problemId}`
  - `POST /api/coding/submissions`
  - `GET /api/coding/submissions/me`
- Added Blazor practice problem list page.
- Added Blazor problem detail/editor page.
- Added local submission result display.
- Added practice navigation.
- Upgraded the practice page into a searchable/filterable problem bank.
- Upgraded the problem page into a two-pane coding IDE workspace.
- Added language selector, reset action, sample checks, run feedback, and recent submission history.
- Added API-level problem filtering by topic, difficulty, and search.
- Expanded starter problem content across HTML, CSS, JavaScript, and C#.
- Reworked AxiForge Web shell into an AxiForge-branded top navigation and left practice navigation.
- Reworked the problem bank into a LeetCode-style practice surface with feature cards, topic cloud, category pills, question rows, progress card, and trending skill tags.
- Enforced login/register before students can view or solve practice problems.
- Updated login/register success flow to land students directly in Practice or the requested protected practice route.
- Rebuilt the AxiForge login/register screen as a product access page with demo fallback and return-route handling.
- Fixed AxiPlus Web practice buttons to call the signed AxiPlus practice-launch API instead of opening stale lesson practice links.
- Added AxiForge `/practice/launch` page to validate AxiPlus launch tokens and continue into login or practice.
- Saved latest UI/flow screenshots under `docs/ui-references`.
- Added signed AxiPlus launch-token exchange so child accounts can auto-enter AxiForge with linked local AxiForge student accounts.
- Added shared launch-token signature validation in AxiForge using `SignedTokens`.
- Reworked Study Plan page toward the provided Binary Search study-plan reference.
- Reworked Coding IDE page toward the provided split description/code/test-result reference.

## Current Local URLs

- AxiForge API: `http://localhost:5055`
- AxiForge Web: `http://localhost:5290`

## Smoke Test Results

| Check | Result |
| --- | --- |
| Full solution build | Pass |
| AxiForge API health | Pass |
| AxiForge student registration | Pass |
| AxiForge student login | Pass |
| Problem list API | Pass |
| Problem filter API | Pass |
| Problem detail API | Pass |
| Authenticated code submission | Pass |
| Submission history API | Pass |
| AxiForge practice page | Pass |
| AxiForge problem editor page | Pass |
| Unauthenticated practice guard | Pass |
| Login to practice return flow | Pass |
| AxiForge login page render | Pass |
| AxiForge practice launch page render | Pass |
| AxiPlus practice-launch endpoint auth gate | Pass |
| Signed AxiPlus child launch-login exchange | Pass |
| Study Plan reference page render | Pass |

## Demo Account

| Field | Value |
| --- | --- |
| Email | `forge.student@axionora.com` |
| Password | `Forge@123` |

## Starter Problems

- HTML Heading Card
- Profile Card Layout
- Sum Two Numbers
- Reverse String
- Array Maximum
- FizzBuzz Lite

## Judge0 Status

The current implementation uses `LocalJudge0Client`, a Judge0-compatible local adapter. It validates submissions against stored test case expected outputs so the product flow works immediately.

Next Judge0 work:

- Add real Judge0 HTTP client.
- Add language ID mapping.
- Add submission token polling.
- Add runtime and memory capture.
- Add compile error and stderr storage.
- Add timeout/error handling.
- Add configuration switch between local mode and Judge0 mode.

## Database Note

`AxiForgeDb` was reset for this phase because the foundation used `EnsureCreatedAsync` before coding tables existed. Before production hardening, replace `EnsureCreatedAsync` with EF Core migrations.

## Next Phase

Continue Phase 3 hardening or move into Phase 5 after the real Judge0 client is added:

- Problem admin CRUD.
- Hidden test cases.
- Editorials and hints.
- Bookmarks.
- Lesson-linked practice sets from AxiPlus launch context.
- Dashboard coding metrics.
