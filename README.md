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
