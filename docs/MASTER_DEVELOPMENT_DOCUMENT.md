# AxiForge Master Development Document

## Part I - Product Vision

### Product Overview

AxiForge is Axionora's career preparation, coding practice, assessment, AI interview, and skill validation platform. It is designed to help learners practice like real developers, prepare for interviews, prove job readiness, and generate verified career evidence for hiring workflows.

AxiForge complements AxiPlus:

- AxiPlus teaches through mentor-led learning.
- AxiForge trains, assesses, validates, and tracks readiness.
- AxiHire consumes verified candidate summaries from AxiForge.

### Vision

To become the trusted career readiness platform that turns learners into verified, job-ready candidates through measurable practice, assessments, AI interviews, and explainable skill intelligence.

### Mission

Help students build, measure, and prove employability through structured practice, AI-powered feedback, mentor validation, and recruiter-ready skill passports.

### Product Goals

- Provide guided coding and interview preparation.
- Measure technical, communication, and career readiness.
- Create traceable, explainable scores for students, mentors, admins, and recruiters.
- Synchronize verified learning and readiness signals into a candidate passport.
- Support AxiPlus students and direct AxiForge subscribers.
- Build a modular platform that can evolve into multi-tenant and white-label offerings.

### Core Positioning

AxiForge is the practice, assessment, and interview-preparation platform that turns learners into verified, job-ready candidates.

### Business Value

- Increases AxiPlus student outcomes through measurable practice and readiness tracking.
- Creates a direct self-learning subscription product.
- Enables premium AI interview, mock interview, and certification revenue.
- Powers AxiHire recruiter verification with trusted candidate evidence.
- Creates defensible historical skill data across the Axionora ecosystem.

### Product Principles

- API first.
- AI first.
- Modular design.
- Explainable scores.
- Historical data is never lost.
- Every meaningful activity updates the passport.
- Every score is traceable.
- Every recommendation is explainable.
- Multi-tenant ready.
- White-label ready.
- Cloud native.
- Mobile ready.
- Audit everything.
- Secure by default.

### Target Audience

- AxiPlus students.
- Direct AxiForge learners.
- Mentors and interviewers.
- Platform admins.
- Recruiters who need verified readiness summaries.
- Colleges or institutions in future white-label deployments.

### User Personas

#### Student

Wants structured practice, confidence, interview readiness, and proof of skills.

Primary needs:

- Daily practice goals.
- Coding problem solving.
- Roadmaps.
- AI interviews.
- Mock interviews.
- Readiness score.
- Feedback and next actions.

#### Mentor

Wants visibility into student practice, weak topics, assessment performance, and interview readiness.

Primary needs:

- Student progress dashboard.
- Weakness analysis.
- Mock interview feedback tools.
- Roadmap and assessment visibility.
- Intervention recommendations.

#### Admin

Wants control over platform content, users, plans, reports, operations, and quality.

Primary needs:

- Content management.
- Problem bank management.
- Assessment management.
- User and subscription management.
- Analytics and audit logs.
- System configuration.

#### Recruiter

Wants trusted candidate summaries without private student history.

Primary needs:

- Verified skill passport.
- Readiness score.
- Assessment summary.
- Interview summary.
- Project and resume signals.

### Success Metrics (KPIs)

- Weekly active students.
- Problems solved per student per week.
- Assessment completion rate.
- AI interview completion rate.
- Mock interview booking and completion rate.
- Career readiness score improvement.
- Coding streak retention.
- Recommendation completion rate.
- Subscription conversion rate.
- Mentor intervention success rate.
- Recruiter verification views.
- Platform uptime and API latency.

## Part II - Platform Architecture

### High-Level Architecture

```text
                               AxiForge Platform
┌─────────────────────────────────────────────────────────────────────┐
│                            Presentation Layer                       │
├─────────────────────────────────────────────────────────────────────┤
│ Student Portal │ Mentor Portal │ Admin Portal │ Recruiter Portal    │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                           Application Layer                         │
├─────────────────────────────────────────────────────────────────────┤
│ Dashboard │ Coding Practice │ Roadmaps │ Assessments │ AI Interviews│
│ Mock Interviews │ Virtual Internship │ AI Coach │ Career Readiness  │
│ Analytics │ Notifications │ Subscription                         │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                           Intelligence Layer                        │
├─────────────────────────────────────────────────────────────────────┤
│ Recommendation Engine │ Skill Graph Engine │ Competency Engine      │
│ Readiness Engine │ Passport Sync │ AI Services │ Risk Detection     │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         Infrastructure Layer                        │
├─────────────────────────────────────────────────────────────────────┤
│ Identity │ Judge0 │ OpenAI / LLM │ Storage │ SignalR │ Redis        │
│ Background Jobs │ PostgreSQL                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### Future Architecture

The first release should be a modular monolith using Clean Architecture. The module boundaries must be designed so high-scale domains can later become services.

Future service candidates:

- Coding execution service.
- Assessment service.
- AI interview service.
- Passport service.
- Notification service.
- Analytics service.
- Subscription service.
- Search service.

### Product Domains

- Identity and access.
- Student learning profile.
- Coding practice.
- Assessments.
- Roadmaps.
- AI interviews.
- Mock interviews.
- Virtual internships.
- Career readiness.
- Competency and skill graph.
- Recommendations.
- Analytics.
- Passport synchronization.
- Notifications.
- Subscription and billing.
- Administration.
- Recruiter verification.

### Core Modules

- Student Dashboard.
- Coding Practice.
- Roadmaps.
- Assessment Engine.
- AI Interview Engine.
- Mock Interviews.
- Virtual Internships.
- AI Career Coach.
- Career Readiness Engine.
- Competency Framework.
- Skill Graph Engine.
- Recommendation Engine.
- Analytics.
- Passport Synchronization.
- Notifications.
- Subscription and Billing.
- Leaderboards.
- Gamification.

### Core Engines

Every major feature should be powered by a dedicated engine:

- Recommendation Engine.
- Career Readiness Engine.
- Competency Engine.
- Skill Graph Engine.
- Assessment Engine.
- AI Interview Engine.
- Roadmap Engine.
- Analytics Engine.
- Passport Sync Engine.
- Notification Engine.
- Subscription Engine.
- Search Engine.
- Ranking Engine.
- Internship Evaluation Engine.

### AI Platform Architecture

AI should be centralized in a single AI Platform layer instead of being embedded directly in every module.

```text
AI Platform
├── AI Coach
├── Interview AI
├── Resume Analyzer
├── Code Reviewer
├── Debug Assistant
├── Recommendation AI
├── Communication Evaluator
├── Roadmap Generator
├── Project Evaluator
├── Skill Gap Analyzer
└── Learning Assistant
```

AI platform responsibilities:

- Provider abstraction.
- Prompt templates and versioning.
- AI request logging.
- Token and cost tracking.
- Safety filtering.
- Structured response parsing.
- Human-readable explanations.
- Evaluation consistency.
- Fallback behavior.

### Integration Architecture

Primary integrations:

- AxiPlus for enrolled students, mentor visibility, and subscription entitlement.
- AxiHire for recruiter-safe verified summaries.
- Judge0 for code execution.
- OpenAI or compatible LLM provider for AI features.
- Email/SMS/WhatsApp providers for notifications.
- Payment provider for subscriptions.
- Cloud object storage for resumes, recordings, and reports.

### API-First Strategy

All product capabilities must be exposed through stable APIs before portal-specific UI assumptions are made.

API design rules:

- REST APIs for standard CRUD and workflows.
- SignalR hubs for real-time events.
- Versioned routes.
- OpenAPI/Swagger documentation.
- DTOs at API boundaries.
- No direct UI dependency on EF entities.
- Idempotent webhook handlers.

### Database Separation Strategy

Initial release uses PostgreSQL with separate databases for common data and each product.

Databases:

- `AxiCoreDb`: shared identity mapping, tenants, entitlements, launch tokens, shared audit, notifications, integration events.
- `AxiPlusDb`: courses, modules, lessons, students, mentors, attendance, assignments, and AxiPlus lesson-practice mappings.
- `AxiForgeDb`: problems, submissions, Judge0 results, roadmaps, assessments, AI interviews, readiness, analytics, and passport events.
- `AxiHireDb`: recruiter invitations, verification links, candidate views, shortlists, notes, and hiring workflows.

Product ownership rules:

- AxiPlus writes to `AxiPlusDb`.
- AxiForge writes to `AxiForgeDb`.
- AxiHire writes to `AxiHireDb`.
- Shared platform services write to `AxiCoreDb`.
- Cross-product communication happens through APIs, contracts, events, and signed launch tokens.

Historical records should be append-friendly. Scores, evaluations, submissions, and passport events must not be overwritten destructively.

### Common Library Strategy

AxiCore must provide separate reusable libraries for shared concerns:

- `AxiCore.SharedKernel`.
- `AxiCore.Contracts`.
- `AxiCore.Identity`.
- `AxiCore.Diagnostics`.
- `AxiCore.Security`.
- `AxiCore.Persistence`.
- `AxiCore.Infrastructure`.

Product-specific code must stay inside the owning product:

- AxiPlus code in `AxiPlus`.
- AxiForge code in `AxiForge`.
- AxiHire code in `AxiHire`.

### Microservice Readiness

The initial system should remain a modular monolith but prepare for service extraction by:

- Keeping domain contracts explicit.
- Avoiding cross-domain table writes outside approved services.
- Publishing domain events.
- Using interfaces for external infrastructure.
- Keeping module-specific migrations organized.
- Avoiding shared mutable state between engines.

### Multi-Tenant / White-Label Readiness

Tenant readiness must be built into the data model early.

Tenant-aware entities should include:

- Tenant ID.
- Created by.
- Updated by.
- Audit metadata.
- Visibility rules.
- Branding configuration where required.

Future tenant capabilities:

- Custom domain.
- Theme and logo.
- Institution-specific problem sets.
- Institution-specific roadmaps.
- Isolated reporting.
- Plan-level feature flags.

## Part III - Functional Modules

### Student Dashboard

Purpose:

Provide the student with the next best action and readiness status.

Features:

- Today's practice goal.
- Coding streak.
- Problems solved.
- Weak topics.
- Upcoming AI interview.
- Upcoming mock interview.
- Career readiness score.
- Recommended next action.
- Passport activity summary.

### Coding Practice

Purpose:

Enable structured coding practice with execution, submissions, feedback, and history.

Features:

- Problem bank.
- Difficulty levels.
- Topic filters.
- Company tags.
- Code editor.
- Language support.
- Test cases.
- Judge0 execution.
- Submissions.
- Accepted and failed attempts.
- Runtime and memory.
- Hints and editorials.
- Bookmarks.

### Roadmaps

Purpose:

Guide students through structured preparation paths.

Features:

- Roadmap templates.
- Student roadmap enrollment.
- Topic sequence.
- Problem sequence.
- Assessment checkpoints.
- AI interview checkpoints.
- Mock interview checkpoints.
- Progress tracking.

### Assessment Engine

Purpose:

Run timed tests and produce traceable scores.

Features:

- DSA tests.
- Technical MCQs.
- Backend tests.
- Frontend tests.
- SQL tests.
- Aptitude tests.
- Communication tests.
- Placement readiness tests.
- Timers.
- Anti-cheat signals.
- Results.
- Weak areas.
- Passport contribution.

### AI Interview Engine

Purpose:

Conduct structured AI interviews and generate explainable feedback.

Features:

- Interview type selection.
- Question generation.
- Text answer support in MVP.
- Voice and video support later.
- Structured scoring.
- Feedback report.
- Improvement plan.
- Historical interview archive.

Scores:

- Technical clarity.
- Communication.
- Confidence.
- Problem-solving.
- Explanation quality.
- Job readiness.

### Mock Interviews

Purpose:

Allow students to book mentor-led interviews and receive validated feedback.

Features:

- Mentor slot management.
- Booking.
- Rescheduling.
- Interview feedback form.
- Score update.
- Career readiness update.
- Plan-based free or paid access.

### Virtual Internships

Purpose:

Simulate real work through projects, weekly tasks, reviews, and evaluations.

Features:

- Internship programs.
- Weekly tasks.
- Project submissions.
- Mentor reviews.
- AI project evaluation.
- Completion certificate.
- Passport contribution.

### AI Career Coach

Purpose:

Recommend the best next action based on student signals.

Features:

- Weakness analysis.
- Practice suggestions.
- Interview preparation guidance.
- Resume suggestions.
- Project explanation coaching.
- Risk detection.

### Career Readiness Engine

Purpose:

Generate a weighted readiness score that summarizes job preparedness.

Inputs:

- Coding score.
- Assessment score.
- AI interview score.
- Mock interview score.
- Consistency.
- Communication.
- Resume completion.
- Project readiness.

Output:

- Readiness score.
- Status.
- Weak dimensions.
- Recommended next actions.
- Passport event.

### Competency Framework

Purpose:

Define skills, levels, competencies, and validation evidence.

Competency examples:

- DSA fundamentals.
- Backend API development.
- Frontend fundamentals.
- SQL querying.
- OOP.
- System design basics.
- Communication.
- Project explanation.

### Skill Graph Engine

Purpose:

Represent relationships between skills, prerequisites, evidence, and readiness.

Features:

- Skill nodes.
- Prerequisites.
- Topic mastery.
- Evidence links.
- Skill gaps.
- Recommended learning paths.

### Recommendation Engine

Purpose:

Select the next best action for each student.

Signals:

- Weak topics.
- Streak.
- Failed submissions.
- Assessment score.
- Interview score.
- Roadmap progress.
- Subscription entitlements.

### Analytics

Purpose:

Track student, mentor, admin, and platform performance.

Metrics:

- Problems solved.
- Daily streak.
- Topic performance.
- Difficulty performance.
- Failed attempts.
- Time spent.
- Interview scores.
- Assessment scores.
- Improvement over time.
- Risk detection.

### Passport Synchronization

Purpose:

Maintain verified career evidence.

Events:

- Problem accepted.
- Assessment completed.
- AI interview completed.
- Mock interview validated.
- Roadmap milestone completed.
- Internship task approved.
- Readiness score updated.

### Notifications

Purpose:

Deliver timely reminders and workflow updates.

Channels:

- In-app.
- Email.
- SMS.
- WhatsApp.
- SignalR real-time updates.

### Subscription & Billing

Purpose:

Control access based on plans, entitlements, purchases, and organization rules.

Features:

- Plans.
- Entitlements.
- Trial access.
- AxiPlus bundled access.
- Direct subscription.
- Paid mock interviews.
- Payment integration.

### Leaderboards

Purpose:

Motivate healthy competition without discouraging beginners.

Types:

- Weekly leaderboard.
- Batch leaderboard.
- College leaderboard.
- Topic leaderboard.
- Streak leaderboard.

### Gamification

Features:

- Streaks.
- Badges.
- Milestones.
- Progress levels.
- Certificates.
- Healthy achievement messaging.

## Part IV - Portals

### Student Portal

Primary areas:

- Dashboard.
- Practice.
- Roadmaps.
- Assessments.
- AI interviews.
- Mock interviews.
- Internship.
- Analytics.
- Passport.
- Subscription.

### Mentor Portal

Primary areas:

- Assigned students.
- Practice activity.
- Weak topics.
- Interview bookings.
- Feedback submission.
- Student readiness trend.
- Intervention recommendations.

### Admin Portal

Primary areas:

- User management.
- Problem bank.
- Roadmaps.
- Assessments.
- AI prompt management.
- Subscription plans.
- Reports.
- Audit logs.
- System settings.

### Recruiter Verification Portal

Primary areas:

- Candidate lookup by shared link or verification code.
- Verified score summary.
- Skills summary.
- Interview summary.
- Assessment summary.
- Project evidence.
- Recruiter-safe passport view.

## Part V - Technical Design

### Database Design Strategy

Use PostgreSQL and EF Core. Model core workflows as domain entities and preserve event history.

Rules:

- Use GUID or UUID identifiers.
- Include tenant, audit, and soft-delete fields where needed.
- Use append-only records for scores, submissions, interviews, and passport events.
- Use read models for analytics dashboards.
- Use migrations per module where practical.

### Entity Relationships

Key relationships:

- Tenant has users, plans, content, and configuration.
- Student has profile, subscriptions, roadmaps, submissions, assessments, interviews, and passport events.
- Problem has topics, test cases, submissions, hints, and editorials.
- Roadmap has milestones, topics, problems, assessments, and checkpoints.
- Assessment has sections, questions, attempts, results, and score dimensions.
- AI interview has session, questions, answers, evaluations, and report.
- Mock interview has booking, mentor, feedback, and readiness contribution.
- Passport has events, evidence, score snapshots, and recruiter-safe summaries.

### API Specifications

Initial API groups:

- `/api/auth`
- `/api/students`
- `/api/dashboard`
- `/api/problems`
- `/api/submissions`
- `/api/roadmaps`
- `/api/assessments`
- `/api/ai/interviews`
- `/api/ai/coach`
- `/api/mock-interviews`
- `/api/readiness`
- `/api/passport`
- `/api/notifications`
- `/api/subscriptions`
- `/api/admin`
- `/api/recruiter-verification`

### Authentication & Authorization

Use ASP.NET Core Identity with JWT/API authentication and cookie authentication for Blazor if required.

Roles:

- Student.
- Mentor.
- Admin.
- Recruiter.
- Super Admin.

Authorization must support:

- Role policies.
- Tenant policies.
- Subscription entitlement policies.
- Resource ownership checks.

### Audit Logs

Audit:

- Login and logout.
- Role changes.
- Subscription changes.
- Score updates.
- Content changes.
- AI prompt changes.
- Recruiter verification access.
- Admin actions.

### AI Prompt Management

Prompt management should include:

- Prompt key.
- Version.
- Feature.
- Template.
- Input schema.
- Output schema.
- Status.
- Created by.
- Approved by.
- Evaluation notes.

### Background Jobs

Suggested jobs:

- Passport synchronization.
- Readiness recalculation.
- Notification dispatch.
- Assessment result aggregation.
- Analytics rollups.
- AI report generation.
- Subscription renewal checks.
- Cleanup of temporary execution artifacts.

### Security

Security requirements:

- Secure by default.
- Tenant isolation.
- Input validation.
- Output encoding.
- Rate limiting.
- Secrets outside source control.
- Encrypted sensitive fields where required.
- Signed recruiter verification links.
- Audit logging.
- Least privilege access.

### Performance

Targets:

- Dashboard API under 500 ms for common requests.
- Problem list API under 300 ms with pagination.
- Submission status updates through SignalR.
- Analytics served from read models.
- Expensive AI/report operations handled asynchronously where appropriate.

### Monitoring

Monitor:

- API latency.
- Error rate.
- Judge0 queue and failures.
- AI provider latency and cost.
- Background job failures.
- Database CPU, memory, locks, and slow queries.
- Redis health.
- SignalR connection counts.

### Logging

Use Serilog with structured logs.

Log:

- Correlation ID.
- User ID where safe.
- Tenant ID.
- Request path.
- Duration.
- Error details.
- External provider timing.

### Backup & Disaster Recovery

Requirements:

- PostgreSQL automated backups.
- Point-in-time recovery.
- Object storage versioning.
- Migration rollback plan.
- Disaster recovery runbook.
- Scheduled restore testing.

## Part VI - Quality

### Test Strategy

Test levels:

- Unit tests for engines and domain rules.
- Integration tests for APIs and EF Core.
- Contract tests for external providers.
- End-to-end tests for critical portal workflows.
- Security tests for authorization and tenant isolation.
- Performance tests for dashboards and submissions.

### Acceptance Criteria

Platform-level acceptance:

- Students can register, log in, access entitled features, solve problems, take assessments, complete AI interviews, and view readiness.
- Mentors can review assigned students and submit mock interview feedback.
- Admins can manage content, users, plans, and reports.
- Recruiters can only see verified, shared candidate summaries.
- Every major activity produces traceable history and passport events.

### Release Strategy

Release stages:

- Internal alpha.
- Mentor pilot.
- AxiPlus student beta.
- Direct student public beta.
- v1 production release.

### Deployment Strategy

Recommended stack:

- .NET 9 Blazor Web App.
- ASP.NET Core APIs.
- PostgreSQL.
- Redis.
- Background worker.
- Docker.
- GitHub Actions.
- Cloud object storage.
- OpenAI-compatible AI provider.
- Judge0 execution service.

### Future Roadmap

- Advanced AI coach.
- Video-based AI interviews.
- Resume analyzer.
- Peer battles.
- College competitions.
- Certification tests.
- Company-specific sheets.
- Advanced recruiter visibility.
- Mobile app.
- Multi-tenant institution deployments.
- White-label platform.

## 7-Day AI Agent Development Roadmap

### Day 1 - Foundation

Architecture:

- Create solution.
- Configure Clean Architecture.
- Configure projects.
- Configure dependency injection.
- Configure EF Core.
- Configure PostgreSQL.
- Configure Identity.
- Configure JWT.
- Configure Serilog.
- Configure Docker.
- Configure Swagger.
- Configure Redis.
- Configure SignalR.
- Configure Judge0 integration.
- Configure AI provider abstraction.
- Configure GitHub Actions.

AI agents:

- Solution Agent.
- Infrastructure Agent.
- Security Agent.
- DevOps Agent.

Deliverable:

- Application runs.
- Authentication works.
- CI/CD works.

### Day 2 - Core Platform

Develop in parallel:

- Student Dashboard.
- User Profile.
- Subscription.
- Notifications.
- File Upload.
- Settings.
- Admin Portal.
- Shared Components.
- UI Theme.
- Navigation.

Deliverable:

- Complete shell application.

### Day 3 - Coding Platform

Develop in parallel:

- Problem Bank.
- Categories.
- Difficulty.
- Code Editor.
- Judge0.
- Submission History.
- Bookmarks.
- Hints.
- Editorials.
- Test Cases.

Deliverable:

- Coding platform complete.

### Day 4 - Assessments & Roadmaps

Develop:

- Roadmap Builder.
- Student Roadmaps.
- Assessments.
- Timers.
- Results.
- Competency Engine.
- Skill Graph.
- Recommendation Engine.

Deliverable:

- Practice ecosystem complete.

### Day 5 - AI Features

Develop:

- AI Coach.
- AI Interviews.
- Communication Evaluation.
- Resume Review.
- Code Review.
- Career Readiness.
- Passport Sync.

Deliverable:

- AI ecosystem complete.

### Day 6 - Internships & Analytics

Develop:

- Virtual Internship.
- Weekly Tasks.
- Mentor Dashboard.
- Mock Interviews.
- Analytics.
- Reports.
- Admin Reports.
- Notifications.

Deliverable:

- Production feature set complete.

### Day 7 - Production Readiness

Focus:

- End-to-end testing.
- Performance optimization.
- Security review.
- Bug fixing.
- Database migration validation.
- Backup strategy.
- Logging verification.
- Monitoring dashboards.
- Production deployment.
- Smoke testing.
- Release candidate validation.

Deliverable:

- Production release candidate v1.0.0.

## AI Agent Team

| Agent | Responsibility |
| --- | --- |
| Solution Architect | Overall architecture and integration |
| Backend Agent | APIs, application logic, CQRS |
| Frontend Agent | Blazor UI and UX |
| Database Agent | Schema, migrations, optimization |
| Authentication Agent | Identity, JWT, permissions |
| Judge0 Agent | Coding execution integration |
| AI Agent | LLM integration, prompts, evaluations |
| Testing Agent | Unit, integration, and end-to-end tests |
| DevOps Agent | Docker, CI/CD, deployment |
| Documentation Agent | API docs, technical documentation, release notes |
