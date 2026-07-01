# AxiCore Engineering Standards

## 1. Function Tracing Standard

Every business, service, controller, API, infrastructure, and integration function should have trace visibility.

Required trace events:

- Entering function.
- Exiting function.
- Exception in function.

Required format:

```text
Entering -> {FileName} -> {FunctionName}
Exiting -> {FileName} -> {FunctionName}
Exception -> {FileName} -> {FunctionName} -> {ExceptionMessage}
```

Preferred implementation:

- Use a shared helper for new code.
- Use `ILogger<T>` for production-grade logs.
- Mirror to `Console.WriteLine` where requested.
- Avoid duplicated hand-written trace strings when a shared helper can provide the same output.

## 2. Try/Catch Standard

Apply try/catch to:

- Controllers and endpoints.
- Application service methods.
- Infrastructure service methods.
- External API calls.
- Database write workflows.
- Background jobs.
- AI provider calls.
- Judge0 calls.

Do not blindly wrap:

- Entity constructors unless they contain real logic.
- DTO property-only classes.
- EF Core configuration methods unless they contain risky logic.
- Auto-generated migrations unless manually changed.
- Trivial getters/setters.

Catch blocks must:

- Log `Exception -> {FileName} -> {FunctionName}`.
- Preserve the original exception.
- Re-throw with `throw;` unless returning a controlled API result.
- Never swallow failures silently.

## 3. Function Comment Standard

Every non-trivial function should include a short XML documentation comment.

The comment should explain:

- What the function is used for.
- What it returns.
- Why it exists.

Example:

```csharp
/// <summary>
/// Gets the student dashboard summary used by the student portal landing page.
/// Returns the student's progress, upcoming items, and readiness signals so the UI can show the next best action.
/// </summary>
```

## 4. Reuse Standard

Do not recreate duplicate functions.

Before adding a new function:

- Search existing services, helpers, and engines.
- Reuse existing functions when behavior matches.
- Extract shared behavior into AxiCore shared helpers when two products need it.
- Keep product-specific rules inside product modules.

## 5. Database Access Standard

Do not use C# `lock` around normal EF Core database operations.

Use these mechanisms instead:

- Scoped DbContext per request.
- EF Core transactions for multi-step writes.
- Optimistic concurrency tokens for conflicting updates.
- Database constraints for uniqueness and integrity.
- Row/version checks for critical updates.
- Background job idempotency keys.
- `SemaphoreSlim` only for rare in-process critical sections that do not replace database correctness.

Database write functions must:

- Log entering and exiting.
- Log exceptions.
- Use transactions when multiple related writes must succeed together.
- Avoid sharing DbContext across threads.
- Save changes once per workflow where practical.

## 6. AxiCore Logging Helper Direction

Create a reusable tracing helper in shared infrastructure:

```text
AxiCore.Shared/
  Diagnostics/
    FunctionTrace.cs
```

New code should use a pattern like:

```csharp
using var trace = FunctionTrace.Enter(_logger, nameof(FileName), nameof(FunctionName));
try
{
    // function body
}
catch (Exception ex)
{
    trace.Exception(ex);
    throw;
}
```

This keeps the required console messages consistent without copying strings across the codebase.

## 7. Existing Code Retrofit Strategy

Existing AxiPlus code should be updated in this order:

1. API controllers and endpoints.
2. Infrastructure services.
3. Application services.
4. Authentication and JWT services.
5. Database seed and integration services.
6. Blazor API client services.
7. Blazor components with non-trivial methods.

Generated migrations, DTO-only models, entities with no behavior, and enum files should not be modified unless needed.

## 8. New Product Rule

All new AxiForge and AxiHire code must follow this standard from the first implementation.

## 9. Database-First Product Data Rule

AxiForge, AxiPlus, and AxiHire must not depend on hardcoded product data for normal workflows.

Database-backed data is required for:

- Coding problems, problem statements, examples, constraints, starter code, testcases, expected outputs, editorials, hints, tags, language support, and company mappings.
- Roadmaps, study plans, levels, modules, lessons, steps, topic mappings, class mappings, and entitlement mappings.
- MCQ questions, options, answers, explanations, assessment templates, timers, scoring rules, and result rules.
- Interview question banks, company collections, role-based packs, and practice sets.
- Subscription plans, product entitlements, white-label tenant configuration, and product feature flags.

Allowed static data:

- UI labels, button text, placeholder text, empty states, loading states, validation messages, and route names.
- Enum-like constants when they represent code-level rules and not editable product content.
- Development-only test fixtures inside test projects.

Admin-managed data must support audit trails, draft/published states, validation, and immediate reflection in student-facing portals after save.

Before moving to any AI, analytics, or release-candidate phase, run a hardcoded-data audit with `rg` and remove static product datasets from Web, API, Application, and Infrastructure layers.

## 10. Shared Library Rule

Common behavior must live in AxiCore shared libraries, not be copied into each product.

Shared libraries:

- `AxiCore.SharedKernel`: base entities, value objects, results, pagination, and common abstractions.
- `AxiCore.Contracts`: cross-product DTOs, integration events, entitlement contracts, and launch token contracts.
- `AxiCore.Identity`: shared identity abstractions, role constants, permission constants, and account-linking contracts.
- `AxiCore.Diagnostics`: function tracing, console logging helpers, correlation IDs, and exception logging helpers.
- `AxiCore.Security`: signing, encryption, token validation, and authorization helpers.
- `AxiCore.Persistence`: EF Core conventions, transaction helpers, audit interceptors, concurrency helpers.
- `AxiCore.Infrastructure`: reusable infrastructure implementations such as Redis, storage, email, and common external clients.

Product-specific behavior must stay in product code:

- AxiPlus LMS behavior stays in `AxiPlus`.
- AxiForge practice, assessment, Judge0, AI interview, and readiness behavior stays in `AxiForge`.
- AxiHire recruiter and hiring workflow behavior stays in `AxiHire`.

Do not move product-specific business rules into shared libraries just because two products call related workflows. Share contracts and abstractions first; share implementation only when the behavior is truly common.

## 11. AxiCore Shared Data Ownership Rule

AxiCore owns shared people, access, and money data across products.

Store these in `AxiCoreDb`:

- Users/accounts.
- Roles and permissions.
- Common student profile.
- Staff/admin identity.
- Product access and entitlement records for AxiPlus, AxiForge, and AxiHire.
- Billing, payments, subscriptions, invoices, and shared payment status.
- Salary slips and staff payment records when they are not product-specific.
- Email confirmation, password reset, login audit, and shared notification records.

Store product activity in the owning product database:

- AxiPlus owns LMS activity: batches, modules, lessons, attendance, assignments, mentor reviews, academic progress, and lesson-practice mappings.
- AxiForge owns practice activity: coding problems, testcases, MCQs, roadmaps, submissions, Judge0 results, readiness, and practice analytics.
- AxiHire owns hiring activity: recruiter invitations, candidate views, verification links, shortlists, recruiter notes, and hiring workflow records.

One person should have one AxiCore identity. Product access must be controlled by normalized product-access or entitlement records such as `UserProductAccess`. Avoid creating duplicate user login rows in each product database and avoid adding product-specific access columns to the user table.

## 12. Common Environment Logging Rule

AxiPlus, AxiForge, AxiHire, and AxiCore services must write to one shared environment log file during local and shared-environment debugging:

```text
Logs/axicore-environment.log
```

Rules:

- Do not create separate product log files for API/Web unless a deployment-specific sink requires it.
- `ILogger` file output should include only `Warning`, `Error`, and `Critical`.
- All `Console.WriteLine` output must be captured into the common environment log file.
- Console lifecycle messages should use this structure:

```text
Entering -> ProductName -> FolderName -> FileName -> FunctionName
Exiting -> ProductName -> FolderName -> FileName -> FunctionName
Exception -> ProductName -> FolderName -> FileName -> FunctionName
```

Current shared implementation:

- `AxiCore.Diagnostics.AxiCoreEnvironmentLogging`
- `AxiCore.Diagnostics.AxiCoreConsoleWriter`
- `AxiCore.Diagnostics.FileLoggerProvider`

New product startup files must call `AddAxiCoreEnvironmentLogging(contentRootPath, productName)` after configuring console/debug providers and warning-level filters.
