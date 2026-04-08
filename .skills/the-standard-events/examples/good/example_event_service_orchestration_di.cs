// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Orchestration Integration, DI Registration, and Startup Activation
// Demonstrates:
//   - Publisher orchestration service consuming the event service (Publish[Entity]Async)
//   - Subscriber orchestration service consuming the event service (SubscribeTo[Entity]Event)
//   - Plural vs singular naming on the orchestration layer
//   - Singleton DI registration for broker and event service
//   - Startup activation in Startup.cs (ASP.NET Core 3/5) and minimal API (Program.cs)

// ---------------------------------------------------------------
// File: IStudentEventOrchestrationService.cs
// Publisher orchestration service interface
// events-004: Decouples publisher from any specific subscriber
// ---------------------------------------------------------------

using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.StudentEvents;

namespace MyProject.Services.Orchestrations.StudentEvents
{
    public interface IStudentEventOrchestrationService
    {
        ValueTask PublishStudentEventAsync(Student student);
    }
}

// ---------------------------------------------------------------
// File: StudentEventOrchestrationService.cs
// Publisher orchestration service: composes the entity, delegates publish to event service.
// events-003 (indirectly): Orchestration knows only I[Entity]EventService -- no broker.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.StudentEvents;
using MyProject.Services.Foundations.StudentEvents;
using MyProject.Services.Foundations.Students;

namespace MyProject.Services.Orchestrations.StudentEvents
{
    public class StudentEventOrchestrationService : IStudentEventOrchestrationService
    {
        private readonly IStudentEventService studentEventService;
        private readonly IStudentService studentService;

        public StudentEventOrchestrationService(
            IStudentEventService studentEventService,
            IStudentService studentService)
        {
            this.studentEventService = studentEventService;
            this.studentService = studentService;
        }

        public async ValueTask PublishStudentEventAsync(Student student)
        {
            Student addedStudent =
                await this.studentService.AddStudentAsync(student);

            // events-021: Delegates to event service Publish[Entity]Async
            await this.studentEventService.PublishStudentAsync(addedStudent);
        }
    }
}

// ---------------------------------------------------------------
// File: ILibraryAccountOrchestrationService.cs
// Subscriber orchestration service interface.
// events-082: Exposes SubscribeTo[Entity]Events (plural) -- the startup activation method.
// ---------------------------------------------------------------

namespace MyProject.Services.Orchestrations.LibraryAccounts
{
    public interface ILibraryAccountOrchestrationService
    {
        // events-082: Plural -- wraps the singular event service SubscribeTo[Entity]Event
        void SubscribeToStudentEvents();
    }
}

// ---------------------------------------------------------------
// File: LibraryAccountOrchestrationService.cs
// Subscriber orchestration service: reacts to student events by creating library accounts.
// events-082: Plural SubscribeToStudentEvents activates the singular event service subscription.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.StudentEvents;
using MyProject.Services.Foundations.LibraryAccounts;
using MyProject.Services.Foundations.StudentEvents;

namespace MyProject.Services.Orchestrations.LibraryAccounts
{
    public class LibraryAccountOrchestrationService : ILibraryAccountOrchestrationService
    {
        private readonly IStudentEventService studentEventService;
        private readonly ILibraryAccountService libraryAccountService;

        public LibraryAccountOrchestrationService(
            IStudentEventService studentEventService,
            ILibraryAccountService libraryAccountService)
        {
            this.studentEventService = studentEventService;
            this.libraryAccountService = libraryAccountService;
        }

        // events-082: Public plural method -- called once at startup to activate the subscription
        public void SubscribeToStudentEvents() =>
            // events-063 (singular): Passes a private handler to the event service
            this.studentEventService.SubscribeToStudentEvent(OnStudentEventReceivedAsync);

        // Private handler: contains the orchestration reaction logic
        private async ValueTask OnStudentEventReceivedAsync(Student student) =>
            await this.libraryAccountService.AddLibraryAccountAsync(student);
    }
}

// ---------------------------------------------------------------
// DI Registration -- Startup.cs style (ASP.NET Core 3/5)
// ---------------------------------------------------------------

// In ConfigureServices(IServiceCollection services):

// events-080: Lifetime must match infrastructure requirements.
// LeVent is in-memory -- singleton is REQUIRED for correctness (subscription state lives in the client instance).
// External infrastructure (e.g., Azure Service Bus) -- follow the client library's guidance (typically singleton
// for connection reuse and to keep message processors alive).
services.AddSingleton<IEventBroker, EventBroker>();

// events-081: Event service lifetime must match or be shorter than the broker it depends on.
services.AddSingleton<IStudentEventService, StudentEventService>();

// Orchestration services registered to match their broker/event service dependencies.
services.AddSingleton<IStudentEventOrchestrationService, StudentEventOrchestrationService>();
services.AddSingleton<ILibraryAccountOrchestrationService, LibraryAccountOrchestrationService>();

// ---------------------------------------------------------------
// Startup Activation -- Startup.cs style (ASP.NET Core 3/5)
// ---------------------------------------------------------------

// In Configure(IApplicationBuilder app, IWebHostEnvironment env):

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

// events-083: Activate every subscribing orchestration service after DI is built
// events-084: Must appear here in Configure -- never inside a controller or service method
app.ApplicationServices
    .GetService<ILibraryAccountOrchestrationService>()
    .SubscribeToStudentEvents();

// ---------------------------------------------------------------
// DI Registration -- Program.cs style (.NET 6+ minimal API)
// ---------------------------------------------------------------

// events-080, events-081: Same infrastructure-driven lifetime requirements apply.
builder.Services.AddSingleton<IEventBroker, EventBroker>();
builder.Services.AddSingleton<IStudentEventService, StudentEventService>();
builder.Services.AddSingleton<IStudentEventOrchestrationService, StudentEventOrchestrationService>();
builder.Services.AddSingleton<ILibraryAccountOrchestrationService, LibraryAccountOrchestrationService>();

var app = builder.Build();

// events-083: Activate after app is built -- app.Services is available here
// events-084: Top-level Program.cs is the startup equivalent for minimal API
app.Services
    .GetService<ILibraryAccountOrchestrationService>()
    .SubscribeToStudentEvents();
