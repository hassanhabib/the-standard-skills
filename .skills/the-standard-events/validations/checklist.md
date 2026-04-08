# The Standard Events -- Validation Checklist

Run this checklist before committing any event service code or approving a PR.
Each item is binary: PASS or FAIL.

---

## ARCHITECTURE

- [ ] **events-001** The CulDeSac pattern is applied -- the event flow is one-way with no synchronous return from subscribers.
- [ ] **events-002** The event service is a foundation service -- no higher-level service depends on it as a dependency of a dependency.
- [ ] **events-003** The event service depends only on IEventBroker -- no other service or broker is injected.
- [ ] **events-005** LeVent is the event infrastructure library -- no other library is used.

---

## BROKER STRUCTURE

- [ ] **events-010** The broker interface is named IEventBroker.
- [ ] **events-011** The broker implementation is named EventBroker.
- [ ] **events-012** Entity-specific implementation members are in EventBroker_[Entity].cs.
- [ ] **events-013** Entity-specific interface members are in IEventBroker_[Entity].cs.
- [ ] **events-014** Each entity has its own LeVentClient<[Entity]> instance in the broker constructor.
- [ ] **events-015** The broker exposes exactly Publish[Entity]Async and SubscribeTo[Entity]Event per entity.
- [ ] **events-016** Broker methods are thin pass-throughs -- no logic exists inside the broker.

---

## SERVICE IMPLEMENTATION

- [ ] **events-020** The service exposes exactly Publish[Entity]Async and SubscribeTo[Entity]Event.
- [ ] **events-021** Publish[Entity]Async is async (ValueTask) and validates before publishing.
- [ ] **events-022** SubscribeTo[Entity]Event is void (synchronous) and validates before subscribing.
- [ ] **events-023** No ILoggingBroker is injected into the event service.
- [ ] **events-024** The event service is declared as internal partial class.
- [ ] **events-025** TryCatch delegates are used to isolate exception handling.

---

## VALIDATION

- [ ] **events-030** Publish validates entity is not null before calling the broker.
- [ ] **events-031** Subscribe validates handler is not null before calling the broker.
- [ ] **events-032** Null entity throws NullEventException (not InvalidEntityException).
- [ ] **events-033** Null handler throws NullEventHandlerException.
- [ ] **events-034** Null checks are circuit-breaking -- no error collection, throws immediately.
- [ ] **events-035** Validation exceptions are wrapped in [Entity]EventValidationException.

---

## EXCEPTION HANDLING

- [ ] **events-040** Two TryCatch delegates exist: ReturningNothingFunction and ReturningValueTaskFunction.
- [ ] **events-041** Unexpected exceptions produce FailedEventServiceException.
- [ ] **events-042** FailedEventServiceException is wrapped in [Entity]EventServiceException.
- [ ] **events-043** No DependencyValidation, Dependency, or CriticalDependency exception categories are present.
- [ ] **events-044** FailedEventServiceException carries the original innerException and Data.

---

## FILE STRUCTURE

- [ ] **events-050** Service is split into three partials: main, .Validations.cs, .Exceptions.cs.
- [ ] **events-051** Broker is split into four files: IEventBroker.cs, IEventBroker_[Entity].cs, EventBroker.cs, EventBroker_[Entity].cs.
- [ ] **events-052** Tests are split into root, Logic.Publish, Logic.Subscribe, Validations.Publish, Validations.Subscribe, Exceptions.Publish, Exceptions.Subscribe.
- [ ] **events-053** Root test file contains only mock declarations, service instantiation, and helpers.

---

## NAMING

- [ ] **events-060** Interface is named I[Entity]EventService.
- [ ] **events-061** Implementation is named [Entity]EventService.
- [ ] **events-062** Publish method is named Publish[Entity]Async.
- [ ] **events-063** Subscribe method is named SubscribeTo[Entity]Event -- NOT ListenTo[Entity]Event.
- [ ] **events-064** Exception namespace is [Namespace].Models.Services.Foundations.[Entities].Exceptions.
- [ ] **events-065** All five exception type names follow the required naming pattern.

---

## DEPENDENCY INJECTION

- [ ] **events-080** IEventBroker and EventBroker are registered with the correct DI lifetime for the event infrastructure in use (singleton for in-memory infrastructure such as LeVent; follow client library guidance for external infrastructure such as Azure Service Bus or EventHighway).
- [ ] **events-081** I[Entity]EventService and [Entity]EventService are registered with a lifetime that matches or is shorter than the IEventBroker lifetime.
- [ ] **events-082** Every subscribing orchestration service exposes SubscribeTo[Entity]Events (plural) as a public activation method.
- [ ] **events-083** SubscribeTo[Entity]Events is called at application startup via app.ApplicationServices.GetService<>() or app.Services.GetService<>() -- not self-activating.
- [ ] **events-084** Startup activation calls appear in Configure() or the minimal API equivalent -- not inside controllers, services, or middleware.

---

## TESTING

- [ ] **events-070** SubscribeTo[Entity]Events happy path is tested first.
- [ ] **events-071** Publish[Entity]Async happy path is tested second.
- [ ] **events-072** Validation failures (null handler, null entity) are tested third.
- [ ] **events-073** Service exceptions (unexpected errors) are tested fourth.
- [ ] **events-074** No loggingBrokerMock is declared in the test class.
- [ ] **events-075** Handler mock is Mock<Func<[Entity], ValueTask>>.
- [ ] **events-076** Every test ends with eventBrokerMock.VerifyNoOtherCalls().
- [ ] **events-077** Times.Once used for expected calls, Times.Never for skipped calls.

---

## RESULT

| Category | PASS / FAIL |
|---|---|
| Architecture | |
| Broker Structure | |
| Service Implementation | |
| Validation | |
| Exception Handling | |
| File Structure | |
| Naming | |
| Dependency Injection | |
| Testing | |

**Overall: PASS only when every row is PASS.**
