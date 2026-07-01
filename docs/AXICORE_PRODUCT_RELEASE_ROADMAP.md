# AxiCore Product Release Roadmap

## 1. Direction

AxiCore will be the shared engineering environment for three products:

- **AxiPlus** - mentor-led learning and LMS platform.
- **AxiForge** - coding practice, assessments, AI interviews, and career readiness platform.
- **AxiHire** - recruiter verification and hiring intelligence platform.

The current workspace should evolve from `AxiForge` into the AxiCore monorepo/workspace. AxiPlus already exists inside the workspace and should be stabilized first. AxiForge should be developed as a separate product solution in the same backend environment. Each product owns its own database, while AxiCore owns common/shared data, shared libraries, identity contracts, entitlements, audit, and integration contracts. AxiHire should be built after passport synchronization and recruiter-safe verification data are stable.

## 2. Confirmed Decisions

- AxiForge has a separate login.
- AxiPlus can redirect students to AxiForge practice for a specific lesson, coding task, or developer practice activity.
- AxiForge phase 1 includes foundation, authentication, student dashboard shell, and coding practice with Judge0.
- AI features come in the next phase.
- UI stack: .NET 9 Blazor Web App with Interactive Server.
- Database: PostgreSQL.
- Databases: `AxiCoreDb`, `AxiPlusDb`, `AxiForgeDb`, and `AxiHireDb`.
- Product architecture: separate products, separate product databases, same backend environment, shared common libraries.
- Existing AxiPlus code should be fixed and reused where sensible.

## 3. Recommended Workspace Structure

```text
AxiCore/
├── AxiPlus/
│   ├── AxiPlus.API/
│   ├── AxiPlus.Application/
│   ├── AxiPlus.Domain/
│   ├── AxiPlus.Infrastructure/
│   └── AxiPlus.Web/
├── AxiForge/
│   ├── AxiForge.API/
│   ├── AxiForge.Application/
│   ├── AxiForge.Domain/
│   ├── AxiForge.Infrastructure/
│   └── AxiForge.Web/
├── AxiHire/
│   ├── AxiHire.API/
│   ├── AxiHire.Application/
│   ├── AxiHire.Domain/
│   ├── AxiHire.Infrastructure/
│   └── AxiHire.Web/
├── AxiCore.Shared/
│   ├── AxiCore.SharedKernel/
│   ├── AxiCore.Contracts/
│   ├── AxiCore.Identity/
│   ├── AxiCore.Diagnostics/
│   ├── AxiCore.Security/
│   ├── AxiCore.Persistence/
│   └── AxiCore.Infrastructure/
├── docs/
├── docker-compose.yml
└── AxiCore.sln
```

## 4. Shared AxiCore Backend Strategy

### Databases

Use four PostgreSQL databases from the beginning:

| Database | Owner | Purpose |
| --- | --- | --- |
| `AxiCoreDb` | AxiCore shared platform | Users/accounts, roles, permissions, common student profile, staff identity, product access, product entitlements, billing, payments, subscriptions, salary slips, tenants, launch tokens, shared audit, shared notifications, integration events |
| `AxiPlusDb` | AxiPlus | LMS data: courses, modules, lessons, batches, attendance, assignments, mentor reviews, lesson practice mappings, AxiPlus academic progress |
| `AxiForgeDb` | AxiForge | Practice data: problems, submissions, Judge0 results, roadmaps, assessments, AI interviews, readiness, passport events |
| `AxiHireDb` | AxiHire | Recruiter data: invited recruiters, verification links, candidate views, shortlists, recruiter notes, hiring workflows |

Rules:

- Product-specific data stays in the product database.
- Shared cross-product data stays in `AxiCoreDb`.
- Products communicate through APIs, contracts, events, and signed launch tokens.
- AxiPlus must not directly write AxiForge tables.
- AxiHire must consume recruiter-safe summaries, not raw private student history.
- AxiCore owns one common user/account identity per person across AxiPlus, AxiForge, and AxiHire.
- Product access should be controlled centrally through product access or entitlement records, not duplicated login tables.

### Common Libraries

AxiCore shared libraries should contain reusable code only:

- `AxiCore.SharedKernel`: base entities, value objects, result types, pagination, time abstractions.
- `AxiCore.Contracts`: cross-product DTOs, events, launch token contracts, entitlement contracts.
- `AxiCore.Identity`: shared identity abstractions, role constants, permission constants, account-linking contracts.
- `AxiCore.Diagnostics`: function tracing, console/log helpers, correlation ID helpers.
- `AxiCore.Security`: signing, token validation, encryption helpers, authorization helpers.
- `AxiCore.Persistence`: common EF Core conventions, transaction helpers, audit interceptors.
- `AxiCore.Infrastructure`: shared implementations for email, storage, Redis, external clients, and background-job primitives.

Product code must stay in product folders:

- AxiPlus course, lesson, attendance, assignment, mentor logic stays under `AxiPlus`.
- AxiForge coding, Judge0, assessment, AI interview, readiness logic stays under `AxiForge`.
- AxiHire recruiter, verification, shortlist, hiring workflow logic stays under `AxiHire`.

### Identity

AxiForge will have separate login, but AxiCore should still use common identity infrastructure to avoid duplicated security logic.

Recommended model:

- Shared users/accounts, roles, permissions, common student profile, staff identity, billing, payments, subscriptions, salary slips, email confirmation, password reset, and product access records in `AxiCoreDb`.
- Product-specific profile extensions and product activity tables in the owning product database.
- Central product-access records for AxiPlus, AxiForge, and AxiHire.
- Required cross-product account linking through AxiCore identity.
- AxiPlus-to-AxiForge redirect token for lesson practice launch.

### AxiPlus to AxiForge Redirect

Flow:

```text
AxiPlus lesson page
    ↓
Student clicks Practice
    ↓
AxiPlus creates signed practice launch token
    ↓
Redirect to AxiForge practice URL
    ↓
AxiForge validates token
    ↓
If student is not logged in, ask login/register
    ↓
Open exact problem, lesson practice set, or developer practice task
```

Required data:

- Source product: AxiPlus.
- Student ID or external student reference.
- Lesson ID.
- Course ID.
- Practice type.
- Target roadmap/problem set.
- Expiration.
- Signature.

## 5. Development Phases

### Phase 0 - Workspace Stabilization

Duration: 1 day.

Dates: June 27, 2026.

Scope:

- Rename/restructure workspace mentally and in docs as AxiCore.
- Review AxiPlus build, dependencies, migrations, and connection strings.
- Identify broken AxiPlus areas.
- Move root docs into AxiCore docs.
- Create common environment plan.
- Configure `AxiCoreDb`, `AxiPlusDb`, `AxiForgeDb`, and `AxiHireDb` in Docker/PostgreSQL.
- Keep AxiPlus data in `AxiPlusDb`; move only shared cross-product data to `AxiCoreDb`.
- Confirm local build and run process.

Deliverable:

- AxiPlus builds locally.
- Four-database AxiCore strategy is applied.
- Root structure is ready for AxiForge development.

### Phase 1 - AxiPlus Fix & Integration Preparation

Duration: 1 day.

Dates: June 28, 2026.

Scope:

- Fix current AxiPlus compile/runtime issues.
- Stabilize API and Blazor Web projects.
- Verify Identity and login.
- Verify PostgreSQL migrations.
- Add product-aware identity foundations.
- Add practice redirect button contracts.
- Add lesson-to-practice mapping model.
- Add signed redirect token service.
- Add AxiPlus endpoint for launching AxiForge practice.

Deliverable:

- AxiPlus runs reliably.
- Students can click Practice from AxiPlus lesson screens.
- Backend can create a secure launch payload for AxiForge.

### Phase 1A - AxiPlus Data Cleanup, Flow Validation & Regression Gate

Duration: 1 day.

Dates: June 29, 2026.

Decision:

- Complete this phase before starting Phase 2.
- Do not proceed to AxiForge foundation while AxiPlus data, auth, dashboard, and lesson flow are still uncertain.

Scope:

- Clean the current local AxiPlus database and remove stale test data.
- Replace broad automatic data seeding with controlled development/test seed scripts.
- Keep only required reference data, roles, demo accounts, and minimal learning content for local smoke testing.
- Verify database migrations from an empty `AxiPlusDb`.
- Verify login and role routing for Student, Admin, MainMentor, AssistantMentor, SuperAdmin, and CollegeCoordinator where applicable.
- Verify student dashboard API and Blazor dashboard rendering.
- Verify profile, billing, modules, lessons, live classes, assignments, attendance, and notifications API calls.
- Verify AxiPlus-to-AxiForge practice launch endpoint returns a signed launch payload.
- Verify that failed API calls show controlled UI errors and do not redirect users incorrectly to login.
- Run API regression smoke tests.
- Run Blazor user-flow regression tests.
- Document known gaps before Phase 2 begins.

Deliverable:

- Clean `AxiPlusDb` can be rebuilt from migrations.
- AxiPlus works without relying on hidden or stale seeded data.
- Full AxiPlus student flow is regression-tested.
- Phase 2 is approved only after the regression checklist passes.

### Phase 2 - AxiForge Foundation

Duration: 2 days.

Dates: June 30, 2026 to July 1, 2026.

Scope:

- Create AxiForge solution/projects.
- Configure .NET 9 Blazor Web App with Interactive Server.
- Configure API/Application/Domain/Infrastructure projects.
- Configure AxiForge PostgreSQL connection to `AxiForgeDb`.
- Configure AxiForge integration connection/client for `AxiCoreDb` shared entitlements and launch tokens.
- Configure AxiForge login/register.
- Configure roles and authorization.
- Configure MudBlazor or shared theme.
- Build student portal shell.
- Build dashboard shell.
- Build profile and settings shell.
- Add subscription/entitlement placeholders.
- Add AxiPlus launch token validation.

Deliverable:

- AxiForge app runs separately.
- AxiForge login works.
- Student dashboard shell exists.
- AxiPlus can redirect into AxiForge with context.

### Phase 3 - AxiForge Coding Practice + Judge0

Duration: 2 days.

Dates: July 2, 2026 to July 3, 2026.

Scope:

- Problem bank.
- Topics and difficulty.
- Test cases.
- Code editor UI.
- Judge0 integration.
- Submission creation.
- Submission result polling.
- Submission history.
- Accepted/wrong answer tracking.
- Runtime and memory storage.
- Lesson-linked practice sets.
- Basic coding analytics.
- Dashboard cards for streak and solved count.

Deliverable:

- AxiForge MVP coding practice is usable.
- AxiPlus lesson practice redirects open the correct AxiForge practice context.

### Phase 4 - AxiForge Roadmaps, Assessments & Release Candidate

Duration: 1 day.

Dates: July 3, 2026.

Scope:

- Roadmap templates.
- Student roadmap enrollment.
- Roadmap progress.
- Timed assessments.
- MCQ and coding assessment support.
- Results.
- Weak topic extraction.
- Competency engine foundation.
- Recommendation engine foundation.

- Practice ecosystem is complete enough for student preparation.
- AxiForge v1.0 release candidate is ready.

### Phase 4A - Dynamic Data Conversion Gate

Duration: 1 to 2 days.

Dates: Before Phase 5.

Scope:

- Remove hardcoded product data from AxiForge student-facing pages.
- Replace static coding problem lists, topic counts, study plans, roadmap cards, MCQ questions, assessment templates, company tags, language lists, and interview collections with database-backed APIs.
- Apply the same rule to AxiPlus and AxiHire: no product workflow should depend on static arrays, hardcoded sample rows, or UI-only fake data.
- Keep only empty states, loading states, validation messages, and UI labels as static text.
- Add EF Core migrations for content tables before adding new dynamic features.
- Ensure AxiForge content supports:
  - Roadmap/study-plan mapping.
  - Company interview question mapping.
  - Language mapping.
  - Topic and difficulty mapping.
  - AxiPlus class, lesson, module, and entitlement mapping.
  - MCQ, coding, testcase, editorial, hint, and solution metadata.
- Add regression tests that prove the app works when the database has no content and when it has admin-created content.

Deliverable:

- AxiForge, AxiPlus, and AxiHire follow a database-first content standard.
- AxiForge student portal renders all practice, roadmap, study-plan, company, topic, language, coding, and MCQ data from `AxiForgeDb` or approved AxiCore integration APIs.
- No static demo content is required for normal product flows.

Exit Criteria:

- `rg` audit confirms no hardcoded product datasets remain in AxiForge, AxiPlus, or AxiHire.
- Student pages show clean empty states when no content exists.
- Student pages immediately reflect new database content without code changes.

### Phase 4B - AxiForge Admin Content Management

Duration: 2 to 3 days.

Dates: After Phase 4A and before Phase 5.

Scope:

- Build AxiForge admin content management as a new `AxiForge` side-menu section inside the existing AxiPlus admin portal.
- Keep the current AxiPlus admin UI location, but add AxiForge content operations there.
- Admins can create and update:
  - Coding problem statements.
  - Testcases and expected outputs.
  - Starter code by language.
  - Editorials, hints, constraints, and solution notes.
  - MCQ questions, options, correct answers, and explanations.
  - Roadmaps, study plans, modules, levels, and steps.
  - Topic, language, difficulty, company, interview, class, batch, module, lesson, and entitlement mappings.
- Admin changes must update database tables and reflect immediately in the AxiForge student portal.
- Add audit logs for every content change.
- Add draft/published status so incomplete content is not visible to students.
- Add validation to prevent publishing broken coding problems, missing testcases, or MCQs without correct answers.
- Add role/permission checks for AxiForge content administrators.

Deliverable:

- Admin users can manage the AxiForge problem bank, MCQ bank, roadmap/study-plan content, testcases, and mappings without developer code changes.
- AxiForge student portal consumes only published admin-managed content.

Exit Criteria:

- Admin creates a coding problem and it appears in AxiForge practice.
- Admin edits a testcase and the change is used by submissions.
- Admin creates an MCQ assessment and it appears in AxiForge assessments.
- Admin maps content to AxiPlus class/lesson context and AxiPlus launch opens the correct AxiForge practice set.
- Admin maps content across AxiPlus class, batch, module, and lesson contexts where needed.

### Phase 4C - Unified Registration and Product Account Linking

Duration: 1 to 2 days.

Dates: After Phase 4B and before Phase 5.

Scope:

- Move shared identity, roles, users, accounts, common student profile, billing, payments, subscriptions, salary slips, and product access into AxiCore ownership.
- Update AxiPlus student registration so it creates:
  - One AxiCore user/account.
  - AxiCore common student profile.
  - AxiCore product access for AxiPlus and AxiForge based on plan/entitlement.
  - AxiPlus LMS-specific student records only where the data is truly LMS-specific.
- Keep direct AxiForge registration enabled for users who only want AxiForge.
- Direct AxiForge registration creates the AxiCore user/account immediately and sends email confirmation for account and product access.
- Store account links and product access in AxiCore-owned tables, not by duplicating lookup logic across products.
- Enforce plan/entitlement rules so a user can log in only to products they are allowed to access.
- Ensure AxiPlus-to-AxiForge launch works for newly registered students without manual AxiForge registration.
- Decide how AxiHire accounts will be created later:
  - Recruiter invitation flow.
  - Admin-created recruiter flow.
  - Candidate verification access through AxiCore identity links.
- Add duplicate-email handling across products.
- Add regression tests for:
  - AxiPlus registration creates linked AxiForge account.
  - Direct AxiForge registration works independently.
  - Existing AxiPlus student can be backfilled into AxiForge.
  - Disabled or unpaid student cannot bypass entitlement through direct AxiForge login.
  - Email confirmation is sent for AxiPlus-created and AxiForge-direct accounts.

Deliverable:

- Registration and login are product-aware but identity-linked.
- Students created from AxiPlus can immediately access AxiForge when entitled.
- Direct AxiForge students can register without requiring AxiPlus.
- One shared AxiCore identity controls product access across AxiPlus, AxiForge, and AxiHire.

Exit Criteria:

- One user can be traced across AxiCore, AxiPlus, and AxiForge.
- Account creation is idempotent and does not create duplicate product users.
- Login, register, launch-login, logout, and forgot-password flows work after database cleanup.

### Phase 5 - AxiForge AI & Career Readiness

Duration: 6 days.

Dates: July 4, 2026 to July 9, 2026.

Scope:

- AI Platform abstraction.
- Prompt management.
- AI interview engine.
- AI coach.
- Communication evaluation.
- Code review assistant.
- Career readiness engine.
- Passport sync engine.
- Explainable recommendations.

Deliverable:

- AxiForge can evaluate interview readiness and generate student guidance.

### Phase 6 - AxiForge Mock Interviews, Internships & Analytics

Duration: 5 days.

Dates: July 10, 2026 to July 14, 2026.

Scope:

- Mentor slot booking.
- Mock interview feedback.
- Virtual internship programs.
- Weekly tasks.
- Mentor dashboard.
- Student analytics.
- Admin reports.
- Notifications.
- Passport event history.

Deliverable:

- AxiForge production feature set is complete.

### Phase 7 - AxiForge Hardening & Public Readiness

Duration: 3 days.

Dates: July 15, 2026 to July 17, 2026.

Scope:

- End-to-end testing.
- Security review.
- Authorization review.
- Performance review.
- Database migration validation.
- Backup and restore validation.
- Logging and monitoring verification.
- Bug fixing.
- Release candidate deployment.

Deliverable:

- AxiForge public release readiness.

### Phase 8 - AxiHire Foundation

Duration: 5 days.

Dates: July 18, 2026 to July 22, 2026.

Scope:

- Create AxiHire solution/projects.
- Configure recruiter portal.
- Configure recruiter auth.
- Define recruiter-safe candidate summary.
- Build passport verification link model.
- Build verification code flow.
- Build recruiter dashboard shell.
- Integrate with AxiForge passport data.

Deliverable:

- AxiHire foundation and recruiter verification flow.

### Phase 9 - AxiHire Candidate Verification & Hiring Workflows

Duration: 6 days.

Dates: July 23, 2026 to July 28, 2026.

Scope:

- Candidate searchable profile.
- Verified readiness summary.
- Skill verification.
- Assessment summary.
- Interview summary.
- Project evidence.
- Recruiter notes.
- Shortlisting.
- Candidate sharing.
- Access controls and audit logs.

Deliverable:

- AxiHire MVP recruiter workflow.

### Phase 10 - AxiHire Hardening & Three-Product Release

Duration: 4 days.

Dates: July 29, 2026 to August 1, 2026.

Scope:

- Full AxiPlus, AxiForge, AxiHire integration testing.
- Cross-product auth and redirects.
- Passport synchronization testing.
- Security and audit review.
- Performance tuning.
- Production deployment checklist.
- Smoke testing.
- Release notes.

Deliverable:

- AxiHire v1.0 release candidate.
- AxiCore three-product integrated release candidate.

### Phase 11 - AxiCore Full Suite Public Release

Duration: 9 days.

Dates: August 2, 2026 to August 10, 2026.

Scope:

- Real user testing.
- Production content loading.
- Final bug fixing.
- Deployment validation.
- Backup and restore validation.
- Security smoke test.
- Performance smoke test.
- Release notes.
- Public release.

Deliverable:

- AxiCore full suite public release.

## 6. Estimated Release Dates

These estimates assume one architect plus AI coding agents working daily, immediate product decisions, limited scope drift, and no major external blocker.

| Product | Release Type | Estimated Date |
| --- | --- | --- |
| AxiPlus | Stabilized integrated release | June 28, 2026 |
| AxiForge | v1.0 release candidate | July 3, 2026 |
| AxiHire | v1.0 release candidate | August 1, 2026 |
| AxiCore | Three-product integrated release candidate | August 1, 2026 |

Public release buffer:

- August 2, 2026 to August 10, 2026 is reserved for full-suite testing, fixes, deployment validation, and content loading.

Recommended public release dates:

| Product | Public Release Target |
| --- | --- |
| AxiPlus stabilized release | June 28, 2026 |
| AxiForge public v1 | July 10, 2026 |
| AxiHire public v1 | August 10, 2026 |
| AxiCore integrated suite | August 10, 2026 |

## 7. Critical User Stories

### AxiPlus

- As a student, I can log in and continue my course.
- As a student, I can click Practice from a lesson and open the correct AxiForge practice context.
- As a mentor, I can see whether a student practiced after a lesson.
- As an admin, I can map lessons to AxiForge practice sets.

## 7A. AxiPlus Feature Development Scope

AxiPlus feature development is tracked as part of AxiCore because AxiForge practice, AxiHire verification, and AxiPlus learning data must work together.

### Stable LMS Core

- Authentication engine:
  - Login.
  - JWT.
  - Role redirect.
  - Password reset.
  - Email verification placeholder or real flow.
  - Account active/inactive enforcement.
  - Safer frontend API error handling.
- User and role engine:
  - Student.
  - Main Mentor.
  - Assistant Mentor.
  - Placement Officer.
  - Trainer.
  - Counsellor.
  - College Coordinator.
  - Admin.
  - SuperAdmin.
  - Permission rules where role checks are too broad.
- Student onboarding engine:
  - Registration review.
  - Profile completion.
  - Document upload metadata.
  - Fee verification state.
  - Batch preference.
  - Specialization selection.
- Batch allocation engine:
  - Capacity check.
  - Auto assign student to available batch.
  - Mentor and assistant mentor assignment workflow.
  - Batch transfer foundation.
- Module system engine:
  - Track/module/lesson management.
  - PDF notes/resources.
  - Live-class-first cleanup.
  - Lesson next/previous navigation.
- Student portal:
  - Dashboard.
  - My modules.
  - Lesson details.
  - Live classes.
  - PDF notes.
  - Assignments.
  - Attendance.
  - Notifications.
  - Support.
- Mentor portal:
  - Main mentor vs assistant mentor permissions.
  - Batch view.
  - Student progress view.
  - Live class scheduling.
  - Assignment publishing and review.
  - Attendance marking.
  - Support responses.
  - Announcements.
- Admin portal:
  - Users.
  - Roles.
  - Tracks.
  - Batches.
  - Students.
  - Modules.
  - Mentor assignment.
  - Support overview.

### Academic Automation

- Progression engine:
  - Completion percentage.
  - Assignment completion rule.
  - Attendance eligibility rule.
  - Project submission rule.
  - Assessment pass rule.
  - Auto unlock next module.
- Examination engine:
  - Exam entity model.
  - Question bank.
  - MCQ exams.
  - Practical/coding exam placeholder.
  - Attempts and retests.
  - Score and pass/fail calculation.
- Academic state engine:
  - Applied.
  - Enrolled.
  - Learning.
  - Assessment Pending.
  - Internship Eligible.
  - Internship Ongoing.
  - Placement Ready.
  - Placed.
  - Completed.
  - Dropped.
  - Suspended.
  - State history.
  - Event-driven transitions.
- Review engine:
  - Risk detection.
  - Mentor review notes.
  - Counselling records.
  - Performance summary.
- Recovery engine:
  - Recovery plans.
  - Extra classes.
  - Recovery assignments.
  - Reassessment.
- Batch reallocation engine:
  - Transfer student.
  - Repeat module.
  - Shift batch.
  - Reallocation history.

### Mentor And Assistant Mentor Operations

- Mentor profile details.
- Salary slip upload/list/download.
- Meeting request workflow.
- Assignment ownership/delegation.
- Attendance discrepancy workflow.
- Student follow-up workflow.
- Student review dashboard.
- Support ticket response workflow.
- Role-specific AM/MM visibility.

### Career, Internship, Placement, Certification

- Internship engine:
  - Eligibility.
  - Allocation.
  - Project assignment.
  - Timesheets.
  - Daily reports.
  - Evaluation.
- Placement engine:
  - Resume profile.
  - Mock interview records.
  - AI interview platform integration.
  - Placement readiness score.
  - Job applications.
  - Placement status tracking.
- Certification engine:
  - Course completion certificate.
  - Internship certificate.
  - Experience letter.
  - Certificate PDF generation.
  - QR verification.
  - Certificate download.

## 7B. AxiForge Feature Development Scope

AxiForge feature development includes:

- Student dashboard.
- Coding problem bank.
- Topic-wise practice.
- Difficulty levels.
- Company tags.
- Code editor.
- Judge0 execution.
- Test cases.
- Submissions.
- Submission history.
- Hints and editorials.
- Bookmarks.
- Roadmaps.
- Assessments.
- Timers.
- Results.
- Competency engine.
- Skill graph engine.
- Recommendation engine.
- Coding analytics.
- AI coach.
- AI interviews.
- Communication evaluation.
- Resume review.
- Code review.
- Career readiness score.
- Passport synchronization.
- Mock interview booking.
- Virtual internships.
- Weekly internship tasks.
- Mentor dashboard.
- Admin reports.
- Notifications.
- Subscription and entitlement access.
- Leaderboards and gamification.

### AxiForge

- As a student, I can register and log in directly to AxiForge.
- As a student, I can see my dashboard, streak, solved count, weak topics, and next action.
- As a student, I can solve coding problems and submit code to Judge0.
- As a student, I can view submission history and results.
- As a student, I can follow a roadmap.
- As a student, I can take assessments.
- As a student, I can attend an AI interview in phase 2.
- As a mentor, I can view student practice and weak topics.
- As an admin, I can manage problems, roadmaps, assessments, and prompts.

### AxiHire

- As a recruiter, I can verify a candidate through a signed link or code.
- As a recruiter, I can view only recruiter-safe candidate summaries.
- As a recruiter, I can see verified skills, readiness score, assessment summary, and interview summary.
- As an admin, I can audit recruiter access.

## 8. Open Questions

These are not blockers for starting development, but they should be answered before the related feature is built.

1. Which payment provider should be used for subscriptions?
2. Should coding problems be manually authored first, imported from CSV, or seeded from admin UI?
3. Do we need college/institution tenancy in v1, or only tenant-ready schema?
4. Should salary slips move to a common AxiCore staff/payroll module immediately, or after AxiPlus payroll is stabilized?

## 9. Suggestions

- Use `AxiCoreDb` only for common/shared data, not product-owned data.
- Use normalized `UserProductAccess` or entitlement rows for product access instead of product-specific columns on the user table.
- Keep separate shared libraries and product libraries from the beginning.
- Keep AxiForge login separate at the UX level, but use shared identity infrastructure internally.
- Build AxiPlus-to-AxiForge practice redirect early because it affects identity, lesson mapping, and analytics.
- Build AxiHire only after passport events from AxiForge are stable.
- Do not make AI the first dependency for AxiForge MVP. Coding practice, dashboard, Judge0, and readiness data should exist first.
- Keep every score and recommendation explainable from v1.
- Add audit logs early; retrofitting audit for education/hiring data is painful.
