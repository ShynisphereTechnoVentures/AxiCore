# Phase 4A, 4B, 4C - AxiForge Dynamic Data, Admin Content, and Registration Plan

## Decision

Before Phase 5 starts, AxiForge must stop relying on hardcoded product data. Coding problems, MCQs, roadmaps, study plans, company interview questions, languages, topics, class mappings, and lesson mappings must come from the database or approved AxiCore integration APIs.

The same standard applies to AxiPlus and AxiHire. Static sample data is not allowed in normal product flows.

## Phase 4A - Dynamic Data Conversion Gate

Goal:

- Convert all AxiForge student-facing practice, roadmap, assessment, MCQ, topic, language, company, and study-plan data to database-backed data.

Required behavior:

- Empty database shows clean empty states.
- Admin-created database content appears without code changes.
- AxiPlus launch context can resolve the correct AxiForge practice content.
- No UI page should require static arrays or fake content to function.

## Phase 4B - AxiForge Admin Content Management

Goal:

- Build AxiForge admin content management as a new `AxiForge` side-menu section inside the existing AxiPlus admin portal.
- Keep the current AxiPlus admin UI location, but add AxiForge content screens there so operators do not need to switch portals for product content management.

Admin must manage:

- Problem statements.
- Testcases.
- Starter code.
- MCQs.
- Roadmaps and study plans.
- Topics, languages, companies, difficulties, classes, lessons, and entitlement mappings.
- Class, batch, module, lesson, and practice-set mappings from AxiPlus to AxiForge.

Required controls:

- Draft/published state.
- Validation before publish.
- Audit logs.
- Role-based admin access.

## Phase 4C - Unified Registration and Product Account Linking

Goal:

- Move shared identity and access control toward AxiCore so users are not duplicated across AxiPlus, AxiForge, and AxiHire.
- When AxiPlus registers a student, create one AxiCore user/account and grant the correct product access for AxiPlus and AxiForge.
- Keep direct AxiForge registration available for AxiForge-only students.
- Direct AxiForge registration creates the AxiCore identity immediately and sends email confirmation for both account creation and product access.

Shared AxiCore ownership:

- Users/accounts.
- Roles and permissions.
- Product access flags or product entitlement rows.
- Common student profile.
- Billing, payments, subscriptions, and invoices.
- Staff/admin identity.
- Salary slips and staff payment records where they are not product-specific.
- Email confirmation and password reset tokens.

Product database ownership:

- AxiPlus owns LMS-specific learning data such as batches, modules, lessons, attendance, assignments, mentor reviews, and AxiPlus academic progress.
- AxiForge owns coding practice, MCQs, roadmaps, testcases, submissions, Judge0 results, readiness, and practice analytics.
- AxiHire owns recruiter invitations, verification links, shortlists, candidate views, and hiring workflow data.

Required behavior:

- AxiPlus student can access AxiForge immediately when entitled.
- Direct AxiForge student can register without AxiPlus.
- Duplicate email handling is idempotent.
- Entitlements prevent unauthorized product access.
- Email confirmation is sent immediately after registration and includes product-access context.

## Recommendation

Build Phase 4A first, then Phase 4B, then Phase 4C.

Reason:

- Dynamic content tables and APIs must exist before admin screens can edit them.
- Admin-managed content should exist before registration/linking is expanded, because newly linked users need real content to land on.
- AI and readiness features should not be built on static content.

## Open Questions

- Which exact billing fields remain in AxiCore versus product-specific billing history?
- Do salary slips belong to a common AxiCore staff/payroll module, or should they remain under AxiPlus until AxiHire staff workflows exist?
- Product access will use a normalized `UserProductAccess` or entitlement table, not boolean columns on the user table.
