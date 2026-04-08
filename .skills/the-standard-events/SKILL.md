---
name: The Standard Events
description: Enforces Standard event-driven architecture using the CulDeSac pattern, event brokers, foundation event services, validation, exception handling, and event service testing.
the standard version: v2.12.0
skill version: v0.1.0.0
---

# The Standard Events

## What this skill is

This skill governs how events are architected, implemented, and tested under The Standard.
It covers the CulDeSac pattern, event brokers, foundation event services, validation, exception mapping, and test structure for publish and subscribe operations.

## Explicit coverage map

This skill explicitly covers:

- The CulDeSac pattern and why it exists in Standard-compliant systems
- Event broker structure and implementation
- Foundation event service implementation: main, Validations, and Exceptions files
- Publish and SubscribeTo operation contracts and naming conventions
- Validation rules for null entity and null event handler
- Exception mapping: Validation and Service exceptions only (no Dependency exceptions)
- File and partial-class structure for brokers, services, and tests
- Test structure: Logic, Validations, and Exceptions per operation
- Naming conventions enforced across the entire events layer
- Orchestration integration: how publisher and subscriber orchestration services consume the event service
- Dependency injection registration: lifetime requirements for the event broker and event service
- Startup activation: how and where SubscribeTo calls must be invoked at application startup

## When to use

Use this skill whenever implementing, reviewing, expanding, or testing event-driven communication between services.
Use it whenever deciding how to publish a domain event, how to subscribe to one, or how to validate and handle failures in an event service.
Use it when determining whether the CulDeSac pattern is appropriate for a given service.

## The CulDeSac pattern

The CulDeSac pattern applies when a service needs to send a domain event outward without expecting a synchronous return from any subscriber.
It is a dead-end in the call graph: data flows in one direction, callers are not blocked waiting for a result, and subscribers are free to react independently.

### Why use events

0. To decouple services across bounded contexts without creating direct service-to-service dependencies.
1. To allow multiple subscribers to react to a single domain event independently.
2. To avoid creating orchestration services that must know about every downstream consumer.
3. To support fire-and-forget flows where the publisher does not need confirmation from subscribers.
4. To enable downstream services to scale and evolve without affecting the publisher.
5. To prevent any single orchestrator from accumulating too many responsibilities over time.

## Event service doctrine

0. Event services are always foundation services.
1. Event services sit at the boundary between the domain model and the event infrastructure.
2. Event services must not depend on other services -- only on the event broker.
3. Event services expose exactly two operations per entity: Publish[Entity]Async and SubscribeTo[Entity]Event.
4. Event services do not inject a logging broker -- they have no logging dependency by design.
5. Event services use two TryCatch delegates to isolate exception handling from business logic.
6. Event services do NOT catch Dependency or CriticalDependency exceptions -- they do not call HTTP or storage APIs.

## Broker implementation doctrine

0. The event broker is the only infrastructure dependency of an event service.
1. The event broker wraps any chosen event infrastructure -- no specific event library is mandated.
2. Each entity gets its own event client instance, typed to that entity, declared in the broker constructor.
3. The broker is split into four files: base interface, entity interface partial, base implementation, entity implementation partial.
4. Broker operations are thin pass-throughs to the underlying event infrastructure -- no logic lives in the broker.

## Orchestration integration

The event service is consumed exclusively by orchestration services.
Controllers, processing services, and other foundation services must never depend on an event service directly.

### Publisher side

An orchestration service that needs to raise a domain event calls `Publish[Entity]Async` on the event service.
The orchestration service owns the composition of the entity and delegates the publish call to the event service.
The publisher orchestration service knows nothing about which services will subscribe -- it only knows it must publish.

### Subscriber side

An orchestration service that needs to react to a domain event exposes a method named `SubscribeTo[Entity]Events` (plural).
That method calls `this.[entity]EventService.SubscribeTo[Entity]Event(handler)` where handler is a private async method.
The handler receives the entity and performs the orchestration reaction -- calling foundation services as needed.
Only the subscriber orchestration service knows what to do with the event; the publisher is unaware of subscribers.

### Naming distinction: plural vs singular

0. The event service method is always singular: `SubscribeTo[Entity]Event`.
1. The orchestration service wrapper method is always plural: `SubscribeTo[Entity]Events`.
2. The plural wrapper exists because the orchestration service may route the event to multiple sub-operations.
3. The plural wrapper is the method called at startup -- never the singular event service method directly.

## Dependency injection and startup activation

### Registration rules

0. All orchestration services that publish or subscribe must be registered in DI.
1. The required DI lifetime for `IEventBroker` and `EventBroker` depends on the event infrastructure in use.
2. For **in-memory event infrastructure** (such as LeVent): singleton is required for correctness -- client instances hold subscription handler registrations in memory; scoped or transient registrations produce fresh instances with no registered handlers, so events are never delivered.
3. For **external event infrastructure** (such as Azure Service Bus or EventHighway): follow the client library's own lifetime recommendations -- subscriptions live in the external service and survive independently of the client instance, but clients typically require singleton lifetime for connection reuse and to keep message processors alive.
4. `I[Entity]EventService` and `[Entity]EventService` must be registered with a lifetime that matches the broker they depend on -- never register the service with a longer lifetime than the broker.
5. When in doubt, prefer singleton -- it is safe for all known event infrastructure and avoids silent subscription failures.

### Startup activation rules

0. Event subscriptions are not self-activating -- they must be explicitly started at application startup.
1. After the DI container is built, every subscribing orchestration service must have its `SubscribeTo[Entity]Events()` method called.
2. The activation call must appear in `Configure()` (Startup.cs style) or its equivalent in a minimal API host setup.
3. In Startup.cs: use `app.ApplicationServices.GetService<I[Entity]OrchestrationService>().SubscribeTo[Entity]Events()`.
4. In minimal API (Program.cs): use `app.Services.GetService<I[Entity]OrchestrationService>().SubscribeTo[Entity]Events()` after `app = builder.Build()`.
5. If the startup activation call is missing, no subscriber will ever receive events -- the subscription is silently never registered.
6. Every subscribing orchestration service must have exactly one startup activation call per subscription.

## Validation rules for event services

### Publish validation

0. Validate the entity is not null before publishing.
1. A null entity must throw NullEventException immediately (circuit-breaking).
2. Null entity validation must not collect further errors -- it breaks immediately.

### Subscribe validation

0. Validate the event handler delegate is not null before subscribing.
1. A null handler must throw NullEventHandlerException immediately (circuit-breaking).
2. Null handler validation must not collect further errors -- it breaks immediately.

## Exception handling rules

0. All validation failures must produce [Entity]EventValidationException wrapping the inner exception.
1. All unexpected exceptions must be wrapped in FailedEventServiceException, then in [Entity]EventServiceException.
2. FailedEventServiceException must carry the original exception as innerException and its Data collection.
3. Event services do not use DependencyValidationException or DependencyException categories.
4. TryCatch delegates must be used to separate exception handling from core logic.

## Naming conventions

0. Interface: I[Entity]EventService.
1. Implementation: [Entity]EventService (internal partial class).
2. Publish operation: Publish[Entity]Async (returns ValueTask, async).
3. Subscribe operation: SubscribeTo[Entity]Event (returns void, synchronous).
4. Subscribe must NEVER be named ListenTo[Entity]Event.
5. Broker interface: IEventBroker (shared), with entity-specific partials in IEventBroker_[Entity].cs.
6. Exception naming: Null[Entity]EventException, Null[Entity]EventHandlerException, Failed[Entity]EventServiceException, [Entity]EventValidationException, [Entity]EventServiceException.

## Test rules for event services

0. Test SubscribeTo[Entity]Events happy path first.
1. Test Publish[Entity]Async happy path second.
2. Test validation failures third (null handler, null entity).
3. Test service exceptions fourth (unexpected errors in both operations).
4. Event service tests must NOT declare a loggingBrokerMock -- there is no logging broker.
5. Subscribe handler mocks must use Mock<Func<[Entity], ValueTask>>.
6. Always end every test with eventBrokerMock.VerifyNoOtherCalls().
7. Use Times.Once for expected calls and Times.Never for calls skipped due to validation failures.
