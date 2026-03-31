---
name: The Standard Testing
description: Enforces Standard TDD discipline, validation testing, exception mapping, controller acceptance tests, and UI component testing.
---

# The Standard Testing

## What this skill is

This skill governs how The Standard is tested, verified, and proven.
It covers test-driven development, validation strategies, exception mapping, unit test structure, controller tests, and UI component tests.

## Explicit coverage map

This skill explicitly covers:

- Foundation-service implementation and validation/testing patterns from the Services chapter
- Structural, logical, external, and dependency validation testing
- Exception mapping and category testing
- Processing-service testing responsibilities
- Orchestration-service call-order testing
- Aggregation-service testing restrictions
- REST controller unit and acceptance tests
- UI component testing for bases, core components, and pages
- The supplied implementation specification sections on unit testing, partial test organization, conventions, AAA, and test order
- TDD FAIL/PASS discipline relevant to test creation and implementation verification

## When to use

Use this skill whenever writing, reviewing, expanding, fixing, or sequencing tests.
Use it whenever deciding what to test first, how to map exceptions in tests, or how to prove a Standard-compliant flow.

## Core testing doctrine

0. Follow TDD.
1. Write the failing test first.
2. Verify the test actually fails.
3. Write the minimum implementation required to pass.
4. Verify the full relevant suite passes.
5. Refactor without changing behavior.
6. Repeat.

## Validation testing rules

### Validation order

0. Structural validations first.
1. Logical validations second.
2. External validations third.
3. Dependency validations when the external resource is the source of the failure.

### Circuit-breaking validations

0. Null checks and other hard-stop guards must break immediately.
1. If continuing would create invalid dereference or meaningless work, stop immediately.

### Continuous validations

0. When multiple fields can be invalid independently, collect them before throwing.
1. Use upsertable exception data.
2. Use dynamic rules with condition + message.
3. Use a validations collector routine.
4. Throw once the collector contains errors.

### Hybrid continuous validations

0. Validate parent objects before validating child properties.
1. Split nested validation into levels to avoid unintended null-reference failures.

## Foundation-service test rules

0. Test the happy path first.
1. Then test structural validations.
2. Then test logical validations.
3. Then test external validations.
4. Then test dependency validations.
5. Then test dependency exceptions.
6. Then test service exceptions.
7. Always verify logging and broker calls.
8. Always verify no unwanted calls occurred.
9. Always keep validation and exception behaviors local and explicit.

## Processing-service test rules

0. Test higher-order logic, not primitive broker details.
1. Validate only what the processing service uses.
2. Test shifters.
   - Example: object -> bool or object -> count.
3. Test combinations.
   - Example: retrieve + add, retrieve + modify, ensure-exists, upsert.
4. Test processing exception mapping from foundation exceptions.

## Orchestration-service test rules

0. Test multi-entity flow combinations.
1. Test mapping/branching between contracts when present.
2. Test call order when the flow depends on order.
3. Prefer natural order when inputs/outputs force sequencing.
4. Use explicit order verification when sequencing is not naturally encoded.
5. Verify orchestration-level exception wrapping and unwrapping.
6. Test normalization outcomes indirectly through dependency shape and resulting behavior.

## Aggregation-service test rules

0. Do not test dependency call order in aggregation services.
1. Do not use mock-sequence style order assertions for aggregation services.
2. Test only basic structural validations and exposure-level aggregation behavior.
3. Aggregation services may multi-call or pass-through; test the contract and exposure abstraction, not orchestration logic.

## Controller and protocol test rules

0. Controllers require unit tests for mapping logic.
1. Unit-test success code mappings.
2. Unit-test validation / dependency / service error mappings.
3. Unit-test security i.e authorization / authentication failure mappings.
3. Acceptance-test every endpoint.
4. Clean up test data after acceptance tests.
5. Emulate external resources not owned by the microservice when running acceptance tests.
6. Integration and end-to-end testing are valid beyond unit + acceptance.

## UI component test rules

### Base components

0. Treat bases as thin wrappers.
1. Test their exposed APIs and wrapper behavior when needed.
2. Do not put business logic into bases.

### Core components

0. Core components are test-driven.
1. Test elements.
   - Existence
   - Properties
   - Actions
2. Existence may be tested by property assignment, searching by id, or general search.
3. Test styles when styles are part of the component contract.
4. Test actions that mutate state, create components, or trigger service calls.
5. Core components should integrate with one and only one view service.

### Pages / containers

0. Pages are simpler route containers.
1. They generally do not require unit tests.
2. They should not contain business logic.

## Unit-test conventions from the supplied implementation profile

0. Mirror partial-class split in tests.
1. Use setup/helpers in the root test file.
2. Split tests into logic, validations, and exceptions files.
3. Use GWT: Given / When / Then.
4. Mock all dependencies.
5. Use readable assertions.
6. Use deep cloning to protect expectation identity.
7. Use randomized data by default.
8. Verify exact dependency calls.
9. End with VerifyNoOtherCalls.
10. Keep naming explicit and scenario-driven.

## Exact test implementation order for foundation-service add routines

When implementing an Add{Entity}Async routine under the implementation profile, follow this order:

0. ShouldAdd{Entity}Async
1. ShouldThrowValidationExceptionOnAddIf{Entity}IsNullAndLogItAsync
2. ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalidAndLogItAsync
3. ShouldThrowDependencyValidationExceptionOnAddIfBadRequestErrorOccursAndLogItAsync
4. ShouldThrowDependencyValidationExceptionOnAddIfConflictErrorOccursAndLogItAsync
5. ShouldThrowCriticalDependencyExceptionOnAddIfUnauthorizedErrorOccursAndLogItAsync
6. ShouldThrowCriticalDependencyExceptionOnAddIfForbiddenErrorOccursAndLogItAsync
7. ShouldThrowCriticalDependencyExceptionOnAddIfNotFoundErrorOccursAndLogItAsync
8. ShouldThrowCriticalDependencyExceptionOnAddIfUrlNotFoundErrorOccursAndLogItAsync
9. ShouldThrowDependencyExceptionOnAddIfInternalServerErrorOccursAndLogItAsync
10. ShouldThrowDependencyExceptionOnAddIfServiceUnavailableErrorOccursAndLogItAsync
11. ShouldThrowCriticalDependencyExceptionOnAddIfHttpRequestErrorOccursAndLogItAsync
12. ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync

For storage-based services, substitute the storage equivalents such as duplicate-key, DbUpdate, and SQL exceptions.

## Testing and exception/localization addendum from the supplied implementation profile

## 8. Unit Testing

Unit tests follow The Standard's **partial-class + three-axis** approach.

### 8.1 Test Class Structure

Each entity's tests mirror the same partial-class split as the service:

| Partial file                                      | Tests                                                  |
| ------------------------------------------------- | ------------------------------------------------------ |
| `{Entity}ServiceTests.cs`                        | Setup, mocks, helpers (`CreateRandom{Entity}`, etc.)   |
| `{Entity}ServiceTests.Logic.{Method}.cs`         | Happy-path / success-case tests                        |
| `{Entity}ServiceTests.Validations.{Method}.cs`   | Validation failure tests                               |
| `{Entity}ServiceTests.Exceptions.{Method}.cs`    | Dependency & service exception tests                   |

### 8.2 Conventions

| Convention           | Detail                                                                                      |
| -------------------- | ------------------------------------------------------------------------------------------- |
| Mocking              | **Moq** — `Mock<IStorageBroker>`, `Mock<IModernApiBroker>`, `Mock<ILoggingBroker>`         |
| Assertions           | **FluentAssertions** — `Should().BeEquivalentTo()`                                         |
| Deep cloning         | **DeepCloner** — to isolate input/expected/actual objects                                   |
| Data generation      | **Tynamix.ObjectFiller** — `Filler<{Entity}>` with custom property setup                  |
| Exception comparison | `Xeption.SameExceptionAs()` via `SameExceptionAs` expression helper                        |
| Test naming          | `Should{Action}Async` / `ShouldThrow{Exception}On{Action}If{Condition}AndLogItAsync`      |
| Verify calls         | Every test verifies broker calls (`Times.Once` / `Times.Never`) and ends with `VerifyNoOtherCalls()` |
| Test framework       | **xUnit** — `[Fact]` for single cases, `[Theory] [InlineData]` for parameterised cases     |

### 8.3 Test Pattern — GWT (Given / When / Then)

```
// given  — build input, configure mocks, construct expected exception
// when   — invoke the service method
// then   — assert result / exception, verify broker interactions
```

### 8.4 Test Implementation Order

Tests **must** be written and committed in the following strict order. This ordering ensures
each category builds upon the prior one:

1. **Happy Path** — `ShouldAdd{Entity}Async`
2. **Structural Validations** — null entity check (`ShouldThrowValidationExceptionOnAddIf{Entity}IsNullAndLogItAsync`)
3. **Logical Validations** — property-level checks using `[Theory] [InlineData]` (`ShouldThrowValidationExceptionOnAddIf{Entity}IsInvalidAndLogItAsync`)
4. **External Dependency Validation Exceptions** — `BadRequest` → `Conflict`
5. **External Critical Dependency Exceptions** — `Unauthorized` → `Forbidden` → `NotFound` → `UrlNotFound`
6. **External Non-Critical Dependency Exceptions** — `InternalServerError` → `ServiceUnavailable`
7. **Transport-Level Exception** — `HttpRequestException`
8. **Catch-All Service Exception** — `Exception`

For storage-based services, steps 4–7 are replaced with the corresponding SQL/EF exceptions
(`DuplicateKeyException`, `DbUpdateException`, `SqlException`).

> **Rule — Test Verification Before Commit:** Each FAIL commit must have the test
> **actually running and failing**. Each PASS commit must have **all** tests
> running and passing. Never commit a FAIL without verifying the test runner
> reports a genuine failure. See [Section 12.1.3 — Commits](#1213-commits) for details.

---

## 9. Key Libraries

| Package                | Purpose                                           |
| ---------------------- | ------------------------------------------------- |
| `Xeption`              | Enhanced exceptions with data aggregation         |
| `EFxceptions`          | EF Core wrapper that throws meaningful exceptions |
| `RESTFulSense`         | HTTP client wrapper for external API brokers      |
| `Moq`                  | Mock framework for unit tests                     |
| `FluentAssertions`     | Readable assertion syntax                         |
| `DeepCloner`           | Value-based deep cloning of test objects           |
| `Tynamix.ObjectFiller` | Random test data generation                       |
| `xunit`                | Unit test framework                               |

---
