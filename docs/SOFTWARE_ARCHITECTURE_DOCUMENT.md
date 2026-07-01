# AxiForge Software Architecture Document

## 1. Introduction

### 1.1 Purpose

This Software Architecture Document defines the architecture for AxiForge, a .NET Blazor and PostgreSQL platform for coding practice, assessments, AI interviews, mock interviews, career readiness, analytics, and verified career passports.

### 1.2 Scope

The architecture covers:

- Blazor presentation layer.
- ASP.NET Core application/API layer.
- Domain engines.
- AI Platform.
- PostgreSQL database strategy.
- External integrations.
- Security, monitoring, deployment, and quality strategy.

### 1.3 Technology Decisions

Default stack:

- .NET 9.
- Blazor Web App.
- ASP.NET Core.
- Clean Architecture.
- Entity Framework Core.
- PostgreSQL.
- ASP.NET Core Identity.
- JWT and cookie authentication.
- Redis.
- SignalR.
- Serilog.
- Docker.
- GitHub Actions.
- Judge0.
- OpenAI-compatible LLM provider.

## 2. Architectural Goals

- Modular monolith first, microservice-ready later.
- API-first application surface.
- AI-first product behavior through a centralized AI Platform.
- Traceable scores and explainable recommendations.
- PostgreSQL database separation by product, with AxiCore shared database for common data.
- Multi-tenant and white-label readiness.
- Secure-by-default implementation.
- Cloud-native deployment.
- Strong observability and auditability.

## 3. System Context

```text
Students ─────────────┐
Mentors ──────────────┤
Admins ───────────────┤
Recruiters ───────────┤
                      ▼
                 AxiForge
                      │
      ┌───────────────┼────────────────┐
      ▼               ▼                ▼
   AxiPlus          AxiHire          Judge0
      │                                │
      ▼                                ▼
 Shared Auth                      Code Execution
                      │
                      ▼
            OpenAI-Compatible AI Provider
```

## 4. Architecture Overview

```text
┌─────────────────────────────────────────────────────────────────────┐
│ Presentation Layer                                                   │
│ Blazor Student, Mentor, Admin, and Recruiter portals                 │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│ API/Application Layer                                                │
│ Controllers, endpoints, CQRS handlers, validation, authorization     │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│ Domain Layer                                                         │
│ Entities, value objects, domain services, engines, domain events     │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│ Infrastructure Layer                                                 │
│ EF Core, PostgreSQL, Redis, SignalR, Judge0, AI provider, storage    │
└─────────────────────────────────────────────────────────────────────┘
```

## 5. Solution Structure

Recommended AxiCore .NET workspace:

```text
AxiCore.sln
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
```

Each product keeps its own Domain, Application, Infrastructure, Web/API, and Worker projects where needed. Shared code belongs only in AxiCore shared libraries.

### AxiForge.Domain

Contains:

- Entities.
- Value objects.
- Domain events.
- Domain services.
- Engine contracts.
- Business rules.

Must not reference infrastructure or UI projects.

### AxiForge.Application

Contains:

- Use cases.
- CQRS handlers.
- DTOs.
- Validators.
- Authorization requirements.
- Interfaces for infrastructure services.

Depends on Domain.

### AxiForge.Infrastructure

Contains:

- EF Core DbContext.
- Migrations.
- Repository implementations where needed.
- Identity implementation.
- Judge0 client.
- AI provider clients.
- Redis.
- Storage.
- Email/SMS integrations.
- Background job infrastructure.

Depends on Application and Domain.

### AxiForge.Web

Contains:

- Blazor components.
- API endpoints/controllers.
- Auth UI.
- Portal routing.
- Shared UI components.
- Swagger/OpenAPI.

Depends on Application and Infrastructure.

### AxiForge.Worker

Contains:

- Passport sync jobs.
- Notification jobs.
- Analytics rollups.
- Readiness recalculation.
- AI report processing.

Depends on Application and Infrastructure.

## 6. Domain Architecture

### 6.1 Domains

- Identity.
- Students.
- Coding.
- Roadmaps.
- Assessments.
- AI.
- Interviews.
- Internships.
- Readiness.
- Competency.
- Recommendations.
- Analytics.
- Passport.
- Notifications.
- Billing.
- Administration.
- Recruiter verification.

### 6.2 Core Entities

Identity:

- ApplicationUser.
- Role.
- Permission.
- Tenant.

Students:

- StudentProfile.
- StudentGoal.
- StudentSkill.
- StudentActivity.

Coding:

- Problem.
- ProblemTopic.
- TestCase.
- Submission.
- SubmissionResult.
- Editorial.
- Hint.

Roadmaps:

- Roadmap.
- RoadmapMilestone.
- RoadmapEnrollment.
- RoadmapProgress.

Assessments:

- Assessment.
- AssessmentQuestion.
- AssessmentAttempt.
- AssessmentResult.
- ScoreDimension.

AI:

- AiPromptTemplate.
- AiPromptVersion.
- AiRequestLog.
- AiEvaluation.

Interviews:

- AiInterviewSession.
- AiInterviewQuestion.
- AiInterviewAnswer.
- AiInterviewReport.
- MockInterviewBooking.
- MockInterviewFeedback.

Readiness:

- ReadinessSnapshot.
- ReadinessContribution.
- ReadinessRule.

Passport:

- PassportProfile.
- PassportEvent.
- PassportEvidence.
- RecruiterVerificationLink.

Billing:

- SubscriptionPlan.
- Subscription.
- Entitlement.
- PaymentTransaction.

## 7. Engine Architecture

Each engine should be a domain/application service with explicit inputs and outputs.

### Recommendation Engine

Consumes analytics, roadmap progress, weak topics, assessment results, and subscriptions.

Produces:

- Next best action.
- Practice suggestions.
- Interview suggestions.
- AxiPlus mentor-training recommendation when needed.

### Career Readiness Engine

Consumes:

- Coding score.
- Assessment score.
- AI interview score.
- Mock interview score.
- Consistency score.
- Communication score.
- Resume completion.
- Project readiness.

Produces:

- Readiness score.
- Readiness status.
- Score breakdown.
- Improvement actions.
- Passport event.

### Assessment Engine

Handles:

- Attempt creation.
- Timer validation.
- Question scoring.
- Result aggregation.
- Weakness extraction.
- Passport contribution.

### AI Interview Engine

Handles:

- Interview session creation.
- Question generation.
- Answer evaluation.
- Score calculation.
- Report generation.
- History.

### Passport Sync Engine

Handles:

- Activity-to-passport mapping.
- Recruiter-safe summaries.
- Evidence validation.
- Historical snapshots.

## 8. AI Platform Architecture

### 8.1 Principles

- AI calls must go through the AI Platform.
- Prompts must be versioned.
- AI outputs must be structured.
- Scores must be explainable.
- AI request and cost metadata must be logged.
- Failures must degrade gracefully.

### 8.2 Components

```text
AI Platform
├── Provider Abstraction
├── Prompt Registry
├── Prompt Renderer
├── Structured Output Parser
├── Safety Guard
├── Cost Tracker
├── AI Request Logger
├── Evaluation Normalizer
└── Feature Services
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

### 8.3 Provider Interface

The provider interface should support:

- Chat completion.
- Structured JSON output.
- Embeddings later.
- Streaming later.
- Retry policies.
- Timeout policies.
- Token usage metadata.

## 9. Data Architecture

### 9.1 PostgreSQL Strategy

Use four PostgreSQL databases for v1:

| Database | Purpose |
| --- | --- |
| `AxiCoreDb` | Shared identity mapping, tenants, entitlements, launch tokens, audit, notifications, integration events |
| `AxiPlusDb` | AxiPlus LMS data |
| `AxiForgeDb` | AxiForge practice, assessment, AI interview, readiness, and passport data |
| `AxiHireDb` | AxiHire recruiter verification and hiring workflow data |

Database ownership rules:

- Each product writes only to its own database.
- Shared common services write to `AxiCoreDb`.
- Cross-product workflows use API calls, contracts, domain events, or signed tokens.
- No product should reach directly into another product's database.

### 9.2 History Strategy

Never destructively overwrite:

- Submissions.
- Assessment attempts.
- Interview answers.
- Interview reports.
- Score snapshots.
- Readiness snapshots.
- Passport events.
- Audit logs.

Mutable records may keep current state, but historical evidence must remain.

### 9.3 Tenant Strategy

Tenant-aware tables should include:

- `TenantId`
- `CreatedAt`
- `CreatedBy`
- `UpdatedAt`
- `UpdatedBy`
- `IsDeleted` where applicable.

## 10. API Architecture

### 10.1 API Style

- REST for standard workflows.
- SignalR for real-time code execution/submission/notification updates.
- OpenAPI for documentation.
- API versioning from the start.

### 10.2 API Groups

- Auth.
- Dashboard.
- Problems.
- Submissions.
- Roadmaps.
- Assessments.
- AI Interviews.
- AI Coach.
- Mock Interviews.
- Internships.
- Readiness.
- Passport.
- Notifications.
- Subscriptions.
- Admin.
- Recruiter Verification.

### 10.3 Error Strategy

APIs should return consistent errors:

- Error code.
- User-safe message.
- Trace ID.
- Validation details.

## 11. Security Architecture

### 11.1 Authentication

Use ASP.NET Core Identity.

Supported auth modes:

- Cookie auth for Blazor portal sessions.
- JWT bearer auth for APIs and future mobile clients.

### 11.2 Authorization

Use policy-based authorization.

Policies:

- Role policy.
- Tenant policy.
- Subscription entitlement policy.
- Resource ownership policy.
- Recruiter verification policy.

### 11.3 Data Protection

Requirements:

- Secrets outside source control.
- HTTPS only.
- Password hashing through Identity.
- Sensitive configuration via environment or secret store.
- Signed recruiter links.
- Rate limiting on public and AI endpoints.

## 12. Integration Architecture

### 12.1 Judge0

Responsibilities:

- Submit code.
- Poll execution.
- Normalize result.
- Store output, runtime, memory, and verdict.
- Publish SignalR status updates.

### 12.2 AxiPlus

Responsibilities:

- Student entitlement sync.
- Mentor-student mapping.
- Learning plan sync.
- Practice analytics visibility.

### 12.3 AxiHire

Responsibilities:

- Recruiter-safe passport summary.
- Candidate verification links.
- Verified skill and readiness summaries.

### 12.4 Notifications

Notification channels:

- In-app.
- Email.
- SMS.
- WhatsApp.

## 13. Background Processing

Use a worker project for asynchronous jobs.

Jobs:

- Submission result polling.
- Passport synchronization.
- Readiness recalculation.
- Notification delivery.
- AI report generation.
- Analytics aggregation.
- Subscription renewal processing.

## 14. Observability

### 14.1 Logging

Use Serilog structured logging.

Include:

- Trace ID.
- User ID where safe.
- Tenant ID.
- Module.
- Duration.
- External service timing.

### 14.2 Metrics

Track:

- API latency.
- Error rate.
- AI token usage.
- AI cost.
- Judge0 execution failures.
- Background job failures.
- Database slow queries.
- SignalR connections.

### 14.3 Audit

Audit all admin, scoring, subscription, identity, and recruiter verification activity.

## 15. Deployment Architecture

### 15.1 Environments

- Local.
- Development.
- Staging.
- Production.

### 15.2 Deployment Units

- Blazor/API app.
- Worker app.
- PostgreSQL.
- Redis.
- Judge0 service or external Judge0 endpoint.
- Object storage.

### 15.3 CI/CD

Pipeline stages:

- Restore.
- Build.
- Unit tests.
- Integration tests.
- Security scan.
- Docker build.
- Migration validation.
- Deploy to staging.
- Smoke tests.
- Promote to production.

## 16. Quality Attributes

### Scalability

- Use pagination.
- Use read models for dashboards.
- Use background jobs for expensive operations.
- Use Redis caching for high-read lookup data.

### Reliability

- Retry external provider calls.
- Timeout AI and Judge0 calls.
- Use idempotent jobs.
- Preserve failed job metadata.

### Maintainability

- Use Clean Architecture.
- Keep domain logic out of Blazor components.
- Keep provider integrations behind interfaces.
- Keep modules independently testable.

### Usability

- Mobile-ready Blazor UI.
- Clear student next actions.
- Explainable scores.
- Minimal friction for practice and interviews.

## 17. Testing Architecture

Tests:

- Domain unit tests.
- Application handler tests.
- Infrastructure integration tests.
- API tests.
- Blazor component tests where appropriate.
- Playwright end-to-end tests.
- Authorization and tenant isolation tests.
- AI output contract tests.

## 18. Initial Implementation Plan

### Phase 1 - Foundation

- Create .NET solution.
- Add projects.
- Configure PostgreSQL.
- Configure Identity.
- Configure logging.
- Configure Swagger.
- Configure Docker.
- Create base domain abstractions.

### Phase 2 - Core Shell

- Student portal shell.
- Mentor portal shell.
- Admin portal shell.
- Recruiter portal shell.
- Navigation and shared layout.
- Auth and role routing.

### Phase 3 - Coding and Practice

- Problem bank.
- Judge0 integration.
- Submissions.
- Roadmaps.
- Assessments.

### Phase 4 - AI and Readiness

- AI Platform abstraction.
- AI interview engine.
- AI coach.
- Career readiness engine.
- Passport sync.

### Phase 5 - Production Readiness

- Tests.
- Security review.
- Performance review.
- Monitoring.
- Backups.
- Deployment.

## 19. Key Architecture Risks

| Risk | Mitigation |
| --- | --- |
| AI scores are inconsistent | Version prompts, normalize scores, store explanations, add human review paths |
| Judge0 latency affects UX | Use async execution, status polling, SignalR updates |
| Database becomes tightly coupled | Use separate product databases, AxiCore shared contracts, APIs, and integration events |
| Multi-tenant retrofitting becomes expensive | Add TenantId and tenant policies from v1 |
| Recruiters see too much student data | Build recruiter-safe passport views only |
| Rapid 7-day build creates quality gaps | Use strict acceptance criteria and Day 7 hardening |

## 20. Architecture Decisions

| Decision | Choice | Rationale |
| --- | --- | --- |
| Primary framework | .NET 9 Blazor Web App | Fits existing Microsoft stack and rapid portal delivery |
| Database | PostgreSQL with `AxiCoreDb`, `AxiPlusDb`, `AxiForgeDb`, and `AxiHireDb` | Product data stays isolated while common platform data remains shared |
| Architecture style | Modular monolith with Clean Architecture | Faster v1 delivery, microservice-ready boundaries |
| AI integration | Central AI Platform | Keeps prompts, cost, safety, and logging controlled |
| Code execution | Judge0 | Proven execution engine for multi-language practice |
| Real-time updates | SignalR | Native .NET integration for submissions and notifications |
| Cache | Redis | Supports dashboard, sessions, rate limits, and jobs |
