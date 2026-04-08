# The Standard Events -- Anti-Patterns

---

## AP-EVENTS-001: Using ListenTo Instead of SubscribeTo

**What it is:** Naming the subscribe operation ListenTo[Entity]Event instead of SubscribeTo[Entity]Event.

**Example:**
```csharp
// VIOLATION: events-063 -- ListenTo is not the standard name
public void ListenToPostEvent(Func<Post, ValueTask> postEventHandler)
{
    this.eventBroker.ListenToPostEvent(postEventHandler);
}
```

**Why harmful:** Inconsistency across codebases makes the intent unclear and breaks tooling, templates, and searchability. The Standard mandates SubscribeTo as the canonical prefix for subscribe operations in event services.

**How to fix:**
```csharp
// events-063: Subscribe is the standard naming convention
public void SubscribeToPostEvent(Func<Post, ValueTask> postEventHandler)
{
    this.eventBroker.SubscribeToPostEvent(postEventHandler);
}
```

---

## AP-EVENTS-002: Injecting a Logging Broker Into an Event Service

**What it is:** Adding ILoggingBroker as a dependency of an event service.

**Example:**
```csharp
// VIOLATION: events-023 -- logging broker must not be injected
public PostEventService(IEventBroker eventBroker, ILoggingBroker loggingBroker)
{
    this.eventBroker = eventBroker;
    this.loggingBroker = loggingBroker;
}
```

**Why harmful:** Event services are intentionally lean. They have no storage or HTTP dependency, and the exception contract (Validation and Service only) does not require logging infrastructure. Adding a logging broker increases coupling, changes the test contract, and breaks the uniformity of the event service pattern.

**How to fix:**
```csharp
// events-023: Only the event broker is injected
public PostEventService(IEventBroker eventBroker) =>
    this.eventBroker = eventBroker;
```

---

## AP-EVENTS-003: Adding Dependency Exception Categories to an Event Service

**What it is:** Catching HttpResponseException, StorageBrokerException, or any Dependency/CriticalDependency exception in an event service TryCatch block.

**Example:**
```csharp
// VIOLATION: events-043 -- event services do not call HTTP or storage
catch (HttpResponseNotFoundException httpResponseNotFoundException)
{
    throw CreateAndLogCriticalDependencyException(httpResponseNotFoundException);
}
```

**Why harmful:** Event services communicate via an in-memory event bus (LeVent), not via HTTP or storage. Adding Dependency exception categories implies infrastructure calls that do not exist in an event service. This adds dead code, misleads maintainers, and inflates the exception surface.

**How to fix:** Remove all Dependency and CriticalDependency catch blocks. Keep only validation and catch-all service exception handling:
```csharp
// events-040, events-041, events-042: Only validation and service exceptions
catch (NullPostEventException nullPostEventException)
{
    throw CreateAndLogValidationException(nullPostEventException);
}
catch (Exception exception)
{
    var failedPostEventServiceException = new FailedPostEventServiceException(
        message: "Failed post event service error occurred, please contact support",
        innerException: exception as Xeption,
        data: exception.Data);

    throw CreateAndLogServiceException(failedPostEventServiceException);
}
```

---

## AP-EVENTS-004: Declaring loggingBrokerMock in Event Service Tests

**What it is:** Adding a Mock<ILoggingBroker> to the test root class of an event service test.

**Example:**
```csharp
// VIOLATION: events-074 -- no logging broker in event service tests
public partial class PostEventServiceTests
{
    private readonly Mock<IEventBroker> eventBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock; // VIOLATION
    private readonly IPostEventService postEventService;

    public PostEventServiceTests()
    {
        this.eventBrokerMock = new Mock<IEventBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>(); // VIOLATION
        ...
    }
}
```

**Why harmful:** The event service does not inject a logging broker, so mocking one will never be called. Its presence implies a verification that cannot happen, and it misleads reviewers into thinking logging is expected. VerifyNoOtherCalls on an unused mock also vacuously passes.

**How to fix:**
```csharp
// events-074: Only the event broker mock is needed
public partial class PostEventServiceTests
{
    private readonly Mock<IEventBroker> eventBrokerMock;
    private readonly IPostEventService postEventService;

    public PostEventServiceTests()
    {
        this.eventBrokerMock = new Mock<IEventBroker>();

        this.postEventService = new PostEventService(
            eventBroker: this.eventBrokerMock.Object);
    }
}
```

---

## AP-EVENTS-005: Placing Business Logic Inside the Event Broker

**What it is:** Adding filtering, transformation, or conditional routing logic inside the event broker methods.

**Example:**
```csharp
// VIOLATION: events-016 -- broker must be a thin pass-through
public ValueTask PublishPostAsync(Post post, string eventName = null)
{
    if (post.IsActive) // VIOLATION: logic in broker
    {
        return this.PostEvents.PublishEventAsync(post, eventName);
    }

    return ValueTask.CompletedTask;
}
```

**Why harmful:** Brokers are infrastructure wrappers. All business logic must live in the service layer. Putting logic in the broker bypasses validation, breaks testability (brokers are not unit-tested for logic), and violates separation of concerns.

**How to fix:**
```csharp
// events-016: Broker is a thin pass-through to LeVent
public ValueTask PublishPostAsync(Post post, string eventName = null) =>
    this.PostEvents.PublishEventAsync(post, eventName);
```

---

## AP-EVENTS-006: Missing VerifyNoOtherCalls in Event Service Tests

**What it is:** Omitting eventBrokerMock.VerifyNoOtherCalls() at the end of event service tests.

**Example:**
```csharp
// VIOLATION: events-076 -- missing VerifyNoOtherCalls
this.eventBrokerMock.Verify(broker =>
    broker.PublishPostEventAsync(inputPost),
        Times.Once);

// MISSING: this.eventBrokerMock.VerifyNoOtherCalls();
```

**Why harmful:** Without VerifyNoOtherCalls(), a future implementation change could add an extra broker call and no test would catch it. Silent regressions are introduced.

**How to fix:**
```csharp
// events-076: Always end with VerifyNoOtherCalls
this.eventBrokerMock.Verify(broker =>
    broker.PublishPostEventAsync(inputPost),
        Times.Once);

this.eventBrokerMock.VerifyNoOtherCalls();
```

---

## AP-EVENTS-007: Using InvalidEntityException Instead of NullEventException

**What it is:** Throwing InvalidEntityException or InvalidEventException when the entity passed to Publish is null.

**Example:**
```csharp
// VIOLATION: events-032 -- wrong exception type for null entity
private static void ValidatePostIsNotNull(Post post)
{
    if (post is null)
    {
        throw new InvalidPostException(message: "Post is invalid."); // VIOLATION
    }
}
```

**Why harmful:** Event services have a distinct exception vocabulary. NullEventException signals a circuit-breaking null check, not a validation of individual fields. Using the wrong exception type breaks the exception hierarchy and makes exception-based routing impossible.

**How to fix:**
```csharp
// events-032: Use NullEventException for null entity in event services
private static void ValidatePostIsNotNull(Post post)
{
    if (post is null)
    {
        throw new NullPostEventException(message: "Post status event handler is null.");
    }
}
```

---

## AP-EVENTS-008: Missing Startup Activation of SubscribeTo

**What it is:** Registering an event service and its subscriber orchestration service in DI but never calling `SubscribeTo[Entity]Events()` at application startup.

**Example:**
```csharp
// VIOLATION: events-083 -- subscription is never activated
// ConfigureServices (Startup.cs)
services.AddSingleton<IEventBroker, EventBroker>();
services.AddSingleton<IStudentEventService, StudentEventService>();
services.AddSingleton<ILibraryAccountOrchestrationService, LibraryAccountOrchestrationService>();

// Configure (Startup.cs) -- MISSING startup activation call
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
    // MISSING: app.ApplicationServices
    //     .GetService<ILibraryAccountOrchestrationService>()
    //     .SubscribeToStudentEvents();
}
```

**Why harmful:** Without the startup activation call, the subscription delegate is never registered with the event broker. The subscriber orchestration service exists in DI but it is deaf -- it will never receive any events. No error is thrown; the failure is completely silent.

**How to fix:**
```csharp
// events-083: Activate every subscription at startup
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.ApplicationServices
        .GetService<ILibraryAccountOrchestrationService>()
        .SubscribeToStudentEvents();
}

// .NET 6+ minimal API equivalent (after app = builder.Build())
app.Services
    .GetService<ILibraryAccountOrchestrationService>()
    .SubscribeToStudentEvents();
```

---

## AP-EVENTS-009: Ignoring Infrastructure Lifetime Requirements for Event Broker or Event Service

**What it is:** Registering `EventBroker` or `[Entity]EventService` with a DI lifetime that conflicts with what the event infrastructure requires.

### For in-memory infrastructure (e.g., LeVent)

**Example:**
```csharp
// VIOLATION: events-080 -- LeVent client instances hold subscription state in memory
services.AddScoped<IEventBroker, EventBroker>();        // VIOLATION
services.AddTransient<IStudentEventService, StudentEventService>(); // VIOLATION
```

**Why harmful:** `EventBroker` holds `LeVentClient<[Entity]>` instances that maintain subscriber handler registrations in memory. A scoped or transient registration produces a new broker instance per request or per resolution. Publishers and the startup activation call operate on different instances -- subscriptions registered at startup exist on a discarded instance and are never seen by the instance that handles a request. Events appear to publish but are silently delivered to no one.

**How to fix:**
```csharp
// events-080, events-081: Singleton required -- in-memory state must be shared
services.AddSingleton<IEventBroker, EventBroker>();
services.AddSingleton<IStudentEventService, StudentEventService>();
```

### For external infrastructure (e.g., Azure Service Bus, EventHighway)

**Example:**
```csharp
// VIOLATION: events-080 -- new processor instances are created per request and never started
services.AddTransient<IEventBroker, AzureServiceBusEventBroker>(); // VIOLATION
```

**Why harmful:** Subscriptions for external infrastructure are handled by long-lived message processors (e.g., `ServiceBusProcessor`). A transient or scoped broker creates a new processor instance per resolution. That processor is never started, so no messages are received. The failure mode differs from in-memory infrastructure -- subscriptions survive in the external service, but nothing is listening to deliver them into the application.

**How to fix:**
```csharp
// events-080, events-081: Follow the client library's lifetime guidance -- typically singleton
// for connection reuse and to keep the message processor alive
services.AddSingleton<IEventBroker, AzureServiceBusEventBroker>();
services.AddSingleton<IStudentEventService, StudentEventService>();
```

---

## AP-EVENTS-010: Calling SubscribeTo Inside a Controller or Service Method

**What it is:** Invoking `SubscribeTo[Entity]Events()` inside a controller action, a service method, or any runtime code path rather than at application startup.

**Example:**
```csharp
// VIOLATION: events-084 -- subscription activated inside a controller action
[HttpPost]
public async ValueTask<ActionResult<Student>> PostStudentAsync(Student student)
{
    // VIOLATION: subscription is re-registered on every POST request
    this.libraryAccountOrchestrationService.SubscribeToStudentEvents();

    Student addedStudent = await this.studentOrchestrationService
        .AddStudentAsync(student);

    return Ok(addedStudent);
}
```

**Why harmful:** Every request re-registers the subscription handler. Most event libraries, including LeVent, allow multiple registrations on the same client -- meaning the handler fires once per registration per event. After ten requests, ten duplicate handlers fire for every event, producing ten duplicate side effects.

**How to fix:**
```csharp
// events-083, events-084: Activate once at startup -- never at runtime
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.ApplicationServices
        .GetService<ILibraryAccountOrchestrationService>()
        .SubscribeToStudentEvents();
}
```
