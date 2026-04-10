# The Standard Events -- Rules

## ARCHITECTURE

**events-001** [ERROR] The CulDeSac pattern must be used when a service needs to publish domain events without expecting a return value from subscribers.
**events-002** [ERROR] Event services are always foundation services -- they sit at the boundary between the domain and the event infrastructure.
**events-003** [ERROR] Event services must not depend on other services -- only on the event broker.
**events-004** [ERROR] Events must be used to decouple services across bounded contexts without creating direct service-to-service dependencies.

## BROKER

**events-010** [ERROR] The event broker interface must be named IEventBroker.
**events-011** [ERROR] The event broker implementation must be named EventBroker (partial class).
**events-012** [ERROR] Entity-specific broker implementation members must be defined in a dedicated partial file: EventBroker_[Entity].cs.
**events-013** [ERROR] Entity-specific broker interface members must be defined in a dedicated partial file: IEventBroker_[Entity].cs.
**events-014** [ERROR] The broker must expose exactly two operations per entity: Publish[Entity]Async and SubscribeTo[Entity]Event.
**events-015** [ERROR] Broker operations must be thin pass-throughs to the underlying event infrastructure -- no business logic lives in the broker.

## SERVICE

**events-020** [ERROR] Event services must implement exactly two operations per entity: Publish[Entity]Async and SubscribeTo[Entity]Event.
**events-021** [ERROR] Publish[Entity]Async must be async (ValueTask) and validate the entity before calling the broker.
**events-022** [ERROR] SubscribeTo[Entity]Event must be synchronous (void) and validate the handler before calling the broker.
**events-023** [ERROR] Event services must NOT inject a logging broker -- event services have no logging dependency.
**events-024** [ERROR] Event services must be internal partial classes.
**events-025** [ERROR] Event services must use TryCatch delegates to separate exception handling from business logic.

## VALIDATION

**events-030** [ERROR] Publish operations must validate the entity is not null before calling the broker.
**events-031** [ERROR] Subscribe operations must validate the event handler is not null before calling the broker.
**events-032** [ERROR] Null entity violations must throw NullEventException -- not InvalidEntityException.
**events-033** [ERROR] Null handler violations must throw NullEventHandlerException.
**events-034** [ERROR] Null checks on entity and handler are circuit-breaking: throw immediately, do not continue or collect further errors.
**events-035** [ERROR] Validation exceptions from event services must be wrapped in [Entity]EventValidationException.

## EXCEPTIONS

**events-040** [ERROR] Event services must use two TryCatch delegates: ReturningNothingFunction (sync) and ReturningValueTaskFunction (async).
**events-041** [ERROR] All unexpected exceptions must be wrapped in FailedEventServiceException before being rethrown.
**events-042** [ERROR] FailedEventServiceException must be wrapped in [Entity]EventServiceException before leaving the service boundary.
**events-043** [ERROR] Event services do NOT catch DependencyValidation, Dependency, or CriticalDependency exceptions -- they do not call HTTP or storage APIs.
**events-044** [ERROR] FailedEventServiceException must carry the original exception as innerException and the original exception's Data collection.

## FILE STRUCTURE

**events-050** [ERROR] The event service must be split into three partial files: main (.cs), .Validations.cs, and .Exceptions.cs.
**events-051** [ERROR] The event broker must be split into four files: IEventBroker.cs, IEventBroker_[Entity].cs, EventBroker.cs, EventBroker_[Entity].cs.
**events-052** [ERROR] Test files must follow the partial split: root, Logic.Publish, Logic.Subscribe, Validations.Publish, Validations.Subscribe, Exceptions.Publish, Exceptions.Subscribe.
**events-053** [ERROR] The root test file must contain only: mock declarations, dependency instantiation, and helper/filler methods.

## NAMING

**events-060** [ERROR] The event service interface must be named I[Entity]EventService.
**events-061** [ERROR] The event service implementation must be named [Entity]EventService.
**events-062** [ERROR] Publish operations must be named Publish[Entity]Async.
**events-063** [ERROR] Subscribe operations must be named SubscribeTo[Entity]Event.
**events-064** [ERROR] The model exception namespace must be [Namespace].Models.Foundations.[Entities].Exceptions.
**events-065** [ERROR] Exception types must follow the naming pattern: Null[Entity]EventException, Null[Entity]EventHandlerException, Failed[Entity]EventServiceException, [Entity]EventValidationException, [Entity]EventServiceException.

## DEPENDENCY INJECTION

**events-080** [ERROR] The DI lifetime of IEventBroker and EventBroker must match the requirements of the event infrastructure in use -- singleton is required for in-memory infrastructure (such as LeVent) where subscription state lives in client instances; for external infrastructure (such as Azure Service Bus or EventHighway), follow the client library's lifetime recommendations, which are typically singleton for connection reuse and message processor lifecycle.
**events-081** [ERROR] I[Entity]EventService and [Entity]EventService must be registered with a lifetime that matches their IEventBroker dependency -- never register the service with a longer lifetime than the broker.
**events-082** [ERROR] Every subscribing orchestration service must expose SubscribeTo[Entity]Events (plural) as the public activation method that wraps the event service's SubscribeTo[Entity]Event (singular).
**events-083** [ERROR] SubscribeTo[Entity]Events must be called at application startup via app.ApplicationServices.GetService<I[Entity]OrchestrationService>() (Startup.cs) or app.Services.GetService<>() (minimal API) -- subscriptions are not self-activating.
**events-084** [ERROR] Startup activation calls must appear in Configure() or the minimal API equivalent -- never inside a controller action, service method, or middleware.

## TESTING

**events-070** [ERROR] Test SubscribeTo[Entity]Events (happy path) first.
**events-071** [ERROR] Test Publish[Entity]Async (happy path) second.
**events-072** [ERROR] Test validation failures third (null handler, null entity).
**events-073** [ERROR] Test service exceptions fourth (unexpected errors in Publish and Subscribe).
**events-074** [ERROR] Event service tests must NOT declare a loggingBrokerMock -- event services have no logging broker.
**events-075** [ERROR] Subscribe handler mocks must be declared as Mock<Func<[Entity], ValueTask>>.
**events-076** [ERROR] Always end every event service test with eventBrokerMock.VerifyNoOtherCalls().
**events-077** [ERROR] Use Times.Once for expected broker calls and Times.Never for broker calls that must not occur due to validation failures.
