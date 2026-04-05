# The Standard Skills

A collection of AI-native skills that operationalize [The Standard](https://github.com/hassanhabib/the-standard) — an open-source software engineering framework governing how software is purposely designed, architecturally structured, rigorously tested, and consistently maintained.

These skills bring The Standard's principles directly into your AI-assisted development workflow, enforcing discipline across the full engineering lifecycle.

---

## What Is The Standard?

[The Standard](https://github.com/hassanhabib/the-standard) is a comprehensive software engineering philosophy built around a single belief: **software should be readable, rewritable, and purposeful above all else.**

It is authored and maintained by [Hassan Habib](https://github.com/hassanhabib) and [The Standard Team](https://github.com/hassanhabib/The-Standard-Team) — a global community of engineers committed to building software with clarity, conviction, and craftsmanship.

The Standard is **all-in or all-out.** It cannot be partially adopted. Every principle reinforces the others, forming a coherent system for engineering excellence.

### Core Principles

| Principle | Description |
|---|---|
| **People-First** | Build for users and future maintainers, not machines |
| **Simplicity** | No Utils, Helpers, or Commons — no excessive inheritance |
| **Autonomous Components** | Every component is self-sufficient with its own validations |
| **No Magic** | Explicit, visible code — no hidden routines or implicit behavior |
| **Rewritability** | Any entry-level engineer must be able to understand and rewrite any component |
| **Mono-Micro** | Monolith structure with a microservice mindset |
| **Airplane Mode** | Software must run fully locally — no mandatory cloud dependency |
| **No Toasters** | Teach by conviction, not by style enforcement |
| **Pass Forward** | Knowledge is shared freely |
| **Readability over Optimization** | Clear code beats clever code |

---

## Skills Overview

This repository contains **five interconnected skills** that together cover the complete engineering lifecycle. `the-standard-core` governs all others — establishing the theoretical foundation that every other skill builds upon.

```
the-standard-core
    ├── the-standard-architecture
    ├── the-standard-code-csharp
    ├── the-standard-testing
    └── the-standard-practices
```

---

### `the-standard-core`

The governing layer for all other skills. Defines the Tri-Nature Theory, the mandatory engineering sequence, and the non-negotiable principles that every Standard-compliant system must follow.

**Mandatory Engineering Sequence:**
1. **Purposing** — Observation → Articulation → Solutioning
2. **Modeling** — Data carriers and operational models
3. **Simulation** — How models interact

**Model Types:**
- **Primary** — Core domain entities (e.g., `Student`, `Post`)
- **Secondary** — Supporting entities (e.g., `Address`, `Tag`)
- **Relational** — Entities linking primaries (e.g., `StudentCourse`)
- **Operational** — Temporary carriers (e.g., `ExceptionMessage`)

No architecture, code, or tests may be written before Purposing and Modeling are complete.

---

### `the-standard-architecture`

Governs how systems are decomposed into layers, components, and dependencies using a strict tri-layer architecture.

**The Three Layers:**

| Layer | Role | Rules |
|---|---|---|
| **Brokers** | Wrap external resources | No business logic, no flow control, infrastructure language (Insert/Select/Update/Delete) |
| **Services** | Contain business logic | Pure business language (Add/Retrieve/Modify/Remove), single responsibility per tier |
| **Exposers** | Expose services externally | Pure mapping only, no business logic, REST controllers |

**Service Tiers:**
- **Foundation Services** — Broker-neighboring, single-entity, one-to-one with broker operations
- **Processing Services** — Higher-order logic (Ensure/Upsert/TryAdd/TryRemove/Verify)
- **Orchestration Services** — Multi-entity flows, 2–3 dependencies maximum (Florance Pattern)
- **Aggregation Services** — Final knot tier, fan-in from multiple services

**Exception Taxonomy:**
```
{Entity}ValidationException
{Entity}DependencyValidationException
{Entity}DependencyException
{Entity}CriticalDependencyException
{Entity}ServiceException
```

**REST Conventions:**
- Routes: `/api/{entities}` (plural)
- POST → `201 Created`, GET → `200 OK`, PUT → `200 OK`, DELETE → `200 OK`
- Validation errors → `400 Bad Request`, Critical errors → `500 Internal Server Error`

---

### `the-standard-code-csharp`

Enforces C# naming, organization, and stylistic conventions as defined by [The Standard CSharp Coding Standard](https://github.com/hassanhabib/CSharpCodingStandard/).

**Key Rules:**

| Element | Convention | Example |
|---|---|---|
| Variables | Full names, no abbreviations | `student` not `s` |
| Collections | Plural nouns, no `List` suffix | `students` not `studentList` |
| Null intent | `no` prefix | `noStudent`, `noChangeCount` |
| Methods | Must contain verbs | `GetStudentsAsync()` not `StudentsAsync()` |
| Async methods | Explicit `Async` suffix | `AddStudentAsync()` |
| Parameters | Contextual names | `(string studentName)` not `(string name)` |
| Fields | `camelCase` with `this.` prefix | `this.studentName` |
| Models | No suffix | `Student` not `StudentModel` |
| Services | Singular | `StudentService` |
| Controllers | Plural | `StudentsController` |

**Implementation-Profile Naming:**

| Element | Pattern | Example |
|---|---|---|
| Broker interface | `I{Resource}Broker` | `IStorageBroker` |
| Broker method | `{Action}{Entity}Async` | `InsertStudentAsync` |
| Service method | `Add{Entity}Async` | `AddStudentAsync` |
| Validation exception | `{Adjective}{Entity}Exception` | `NullStudentException` |
| Outer exception | `{Entity}{Category}Exception` | `StudentValidationException` |
| Test method | `Should{Action}Async` | `ShouldAddStudentAsync` |

---

### `the-standard-testing`

Governs TDD discipline, test structure, validation testing patterns, exception mapping, and the precise sequence of test implementation.

**The TDD Cycle:**
1. Write a failing test
2. Verify it actually fails (red)
3. Write minimum implementation to pass
4. Verify full suite passes (green)
5. Refactor without changing behavior
6. Repeat

**Test File Organization (3-axis pattern):**
```
{Entity}ServiceTests.cs                         — Setup, mocks, helpers
{Entity}ServiceTests.Logic.{Method}.cs          — Happy-path tests
{Entity}ServiceTests.Validations.{Method}.cs    — Validation failure tests
{Entity}ServiceTests.Exceptions.{Method}.cs     — Dependency & service exception tests
```

**Test Naming:**
- Happy path: `ShouldAddStudentAsync`
- Failure path: `ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync`

**Required Libraries:**

| Library | Purpose |
|---|---|
| `xUnit` | Test framework |
| `Moq` | Mocking |
| `FluentAssertions` | Readable assertions |
| `DeepCloner` | Isolate test objects |
| `Tynamix.ObjectFiller` | Randomized data generation |
| `Xeption` | Standard-compliant exception comparison |
| `RESTFulSense` | HTTP status-typed exceptions |
| `EFxceptions` | EF Core exception mapping |

Every test ends with `VerifyNoOtherCalls()` to prevent unexpected side effects.

---

### `the-standard-practices`

Governs team workflow: branching conventions, commit discipline, pull request standards, project structure, and configuration management.

**Branch Naming:**
```
users/[username]/[CATEGORY]-[entity]-[action]
```

**Category System (35+ categories):**
`INFRA` · `DATA` · `BROKERS` · `FOUNDATIONS` · `PROCESSINGS` · `ORCHESTRATIONS` · `COORDINATIONS` · `MANAGEMENTS` · `AGGREGATIONS` · `CONTROLLERS` · `VIEWS` · `BASES` · `COMPONENTS` · `PAGES` · `ACCEPTANCE` · `INTEGRATION` · `CODE RUB` · `FIXES` · `DOCUMENTATION` · `CONFIG` · `DESIGN` · `BUSINESS` · `PLANNING` · `MENTORSHIP` · `DISCUSSION` · `REVIEW` · `STANDARD` · `IMPORT` · `STATUS` · and more

**Commit Discipline:**
- Alternating `FAIL` / `PASS` commit pattern enforces true TDD
- Every `FAIL` commit must demonstrate a genuinely failing test
- Every `PASS` commit must show the full suite passing

**PR Title Format:**
```
[CATEGORY]: [Entity] - [Description]
```

**PR Size Classification:**

| Size | Criteria |
|---|---|
| MAJOR | 5+ tests |
| MEDIUM | 3–4 tests |
| MINOR | 1–2 tests |
| N/A | Non-test changes |

**Canonical Project Structure:**
```
{Project}.Core/
├── Brokers/
│   ├── DateTimes/
│   ├── Loggings/
│   └── Storages/
├── Models/
│   └── Foundations/
│       └── {Entity}/
│           └── Exceptions/
├── Services/
│   ├── Foundations/
│   ├── Processings/
│   └── Orchestrations/
└── Controllers/
```

---

## Walkthroughs

These walkthroughs show the complete output an agent should produce when these skills are active. Each one covers code, naming, file structure, and — when the project is source-controlled — branching and commit discipline.

---

### Walkthrough 1 — Develop a Logging Broker

**Prompt to the agent:**
> "Build a LoggingBroker according to The Standard."

**Step 1 — Create a branch** *(source-controlled projects only)*

```bash
git checkout -b users/{username}/BROKERS-logging-create
```

**Step 2 — Create the interface** `Brokers/Loggings/ILoggingBroker.cs`

```csharp
// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

namespace MyProject.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        ValueTask LogInformationAsync(string message);
        ValueTask LogTraceAsync(string message);
        ValueTask LogDebugAsync(string message);
        ValueTask LogWarningAsync(string message);
        ValueTask LogErrorAsync(Exception exception);
        ValueTask LogCriticalAsync(Exception exception);
    }
}
```

**Step 3 — Create the implementation** `Brokers/Loggings/LoggingBroker.cs`

```csharp
// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

namespace MyProject.Brokers.Loggings
{
    public class LoggingBroker : ILoggingBroker
    {
        private readonly ILogger<LoggingBroker> logger;

        public LoggingBroker(ILogger<LoggingBroker> logger) =>
            this.logger = logger;

        public async ValueTask LogInformationAsync(string message) =>
            this.logger.LogInformation(message);

        public async ValueTask LogTraceAsync(string message) =>
            this.logger.LogTrace(message);

        public async ValueTask LogDebugAsync(string message) =>
            this.logger.LogDebug(message);

        public async ValueTask LogWarningAsync(string message) =>
            this.logger.LogWarning(message);

        public async ValueTask LogErrorAsync(Exception exception) =>
            this.logger.LogError(exception, exception.Message);

        public async ValueTask LogCriticalAsync(Exception exception) =>
            this.logger.LogCritical(exception, exception.Message);
    }
}
```

> **Anti-pattern — never do this:**
> ```csharp
> // WRONG: Task.Run() adds thread-pool overhead for a synchronous ILogger<T> call
> public ValueTask LogWarningAsync(string message) =>
>     new ValueTask(Task.Run(() => this.logger.LogWarning(message)));
> ```

**Step 4 — Register in `Program.cs`**

```csharp
builder.Services.AddLogging();
builder.Services.AddTransient<ILoggingBroker, LoggingBroker>();
```

**Step 5 — Commit** *(source-controlled projects only)*

```
BROKERS: LoggingBroker - Add logging broker and interface
```

**Step 6 — Pull Request (source-controlled projects only)**

```bash
git push -u origin users/{username}/BROKERS-logging-create
gh pr create --base main --head users/{username}/BROKERS-logging-create --title "BROKERS: LoggingBroker - Add logging broker and interface" --body "Closes #{issue number}"
```

---

### Walkthrough 2 — Develop a Foundation Service for Student (with TDD)

**Prompt to the agent:**
> "Build a Foundation Service for Student according to The Standard."

The agent executes a strict **FAIL → PASS** commit cycle for every test. Each `FAIL` commit contains a genuinely failing test. Each `PASS` commit contains minimum implementation that makes the full suite green.

**Step 1 — Create a branch** *(source-controlled projects only)*

```bash
git checkout -b users/{username}/FOUNDATIONS-student-create
```

**Step 2 — Scaffold the partial files**

```
Services/Foundations/Students/
├── IStudentService.cs
├── StudentService.cs                  (partial — constructor, public methods)
├── StudentService.Validations.cs      (partial — Validate* and IsInvalid* methods)
└── StudentService.Exceptions.cs       (partial — TryCatch delegate, CreateAndLog* helpers)

Tests.Unit/Services/Foundations/Students/
├── StudentServiceTests.cs             (partial — setup, mocks, helpers)
├── StudentServiceTests.Logic.Add.cs
├── StudentServiceTests.Validations.Add.cs
└── StudentServiceTests.Exceptions.Add.cs
```

**Step 3 — FAIL/PASS cycle (one commit pair per test)**

```
FAIL: ShouldAddStudentAsync
PASS: ShouldAddStudentAsync

FAIL: ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync
PASS: ShouldThrowValidationExceptionOnAddIfStudentIsNullAndLogItAsync

FAIL: ShouldThrowValidationExceptionOnAddIfStudentIsInvalidAndLogItAsync
PASS: ShouldThrowValidationExceptionOnAddIfStudentIsInvalidAndLogItAsync

FAIL: ShouldThrowDependencyValidationExceptionOnAddIfStudentAlreadyExistsAndLogItAsync
PASS: ShouldThrowDependencyValidationExceptionOnAddIfStudentAlreadyExistsAndLogItAsync

FAIL: ShouldThrowDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync
PASS: ShouldThrowDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync

FAIL: ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync
PASS: ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync
```

> **Rule — FAIL commits must be verified red before committing.**
> Run the test suite and confirm the new test fails. Never commit a `FAIL` without
> seeing the test runner report a genuine failure. Never commit a `PASS` without
> seeing the full suite green.

**Step 4 — Pull Request** *(source-controlled projects only)*

```bash
git push -u origin users/{username}/FOUNDATIONS-student-create
gh pr create --base main --head users/{username}/FOUNDATIONS-student-create --title "FOUNDATIONS: Student - Add foundation service with Add operation" --body "Closes #{issue number}"
```

---

## Agent Compliance Model

The skills in this repository use two complementary instruction styles. Understanding the distinction helps you extend or troubleshoot them.

### Declarative rules — describe *what* compliance looks like

```json
{ "id": "arch-009", "description": "All broker methods must be asynchronous." }
```

Rules validate outputs. An agent checks generated code against them. They work well for structural properties that can be inspected at rest (naming, layer boundaries, method signatures).

### Procedural sequences — describe *how* to achieve compliance

```
1. Write the failing test.
2. Run the suite — verify RED. Stop if it does not fail.
3. Write minimum implementation.
4. Run the suite — verify GREEN. Stop if any test fails.
5. Commit PASS.
6. Move to the next test.
```

Procedures govern workflows that have *ordering* and *checkpoints*. The FAIL/PASS commit cycle, branch creation, and PR formatting all require an agent to execute steps in sequence, not just check a property.

### Conditional guards — apply practices only when applicable

Git and commit practices apply **if and only if the project is source-controlled.** The practices skill activates these procedures only when a `.git` directory is detected (or equivalent VCS metadata exists). Without source control, the agent produces code only — no branch, no commits, no PR.

```
IF .git exists:
  → Create branch following users/{username}/{CATEGORY}-{entity}-{action}
  → Alternate FAIL/PASS commits per test
  → Format PR title as [CATEGORY]: [Entity] - [Description]
ELSE:
  → Generate code only
```

### Why this matters

An agent that knows only declarative rules can generate correct code but produce it all in one shot with no branch and no FAIL/PASS history. An agent that also follows the procedural sequences produces an auditable engineering trail — one that looks like it was built by a disciplined team, commit by commit.

Both instruction styles are required to fully operationalize The Standard.

---

## Skill Structure

Each skill is a self-contained, composable package following a consistent layout:

```
skill-name/
├── manifest.json          — Metadata, inputs/outputs, dependencies
├── SKILL.md               — Human-readable guide (250–1000 lines)
├── contracts/
│   └── contracts.json     — Machine-readable validation contracts
├── rules/
│   ├── rules.md           — Numbered canonical rules
│   └── rules.json         — Machine-readable rules
├── examples/
│   ├── good/              — Compliant reference examples
│   └── bad/               — Common violations to avoid
├── templates/             — Reusable starter templates
└── validations/
    ├── checklist.md       — Pre-implementation validation checklist
    └── anti-patterns.md   — Documented violations and corrections
```

---

## Installation

Run this command to install the skills globally or scoped to your solution/project

```bash
npx skills add hassanhabib/the-standard-skills
```
---

## References

These skills are directly derived from and governed by the following official sources:

- **The Standard** — Core theory, modeling, architecture, and principles
  [https://github.com/hassanhabib/the-standard](https://github.com/hassanhabib/the-standard)

- **The Standard Team** — The global community maintaining and evolving The Standard
  [https://github.com/hassanhabib/The-Standard-Team](https://github.com/hassanhabib/The-Standard-Team)

- **The Standard CSharp Coding Standard** — C# naming, style, and code organization rules
  [https://github.com/hassanhabib/CSharpCodingStandard](https://github.com/hassanhabib/CSharpCodingStandard/)

---

## License

This project is licensed under the **The Standard Software License (TSSL)**.
[https://github.com/hassanhabib/The-Standard-Software-License](https://github.com/hassanhabib/The-Standard-Software-License)
