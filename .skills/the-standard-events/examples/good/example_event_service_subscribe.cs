// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Event Service -- Subscribe Operation
// Demonstrates: broker interface, broker implementation, and subscribe tests.

// ---------------------------------------------------------------
// File: IEventBroker.cs
// events-010: Base interface named IEventBroker
// ---------------------------------------------------------------

namespace MyProject.Brokers.Events
{
    // events-010: Shared base interface -- empty, extended by entity partials
    public partial interface IEventBroker
    { }
}

// ---------------------------------------------------------------
// File: IEventBroker_Post.cs
// events-013: Entity-specific interface partial
// events-015: Exactly Publish and SubscribeTo per entity
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.PostEvents;

namespace MyProject.Brokers.Events
{
    public partial interface IEventBroker
    {
        // events-015: Publish and SubscribeTo -- no other operations
        ValueTask PublishPostEventAsync(Post post, string eventName = null);
        void SubscribeToPostEvent(Func<Post, ValueTask> postEventHandler, string eventName = null);
    }
}

// ---------------------------------------------------------------
// File: EventBroker.cs
// events-011: Implementation named EventBroker
// events-014: LeVentClient<Post> instantiated in constructor
// ---------------------------------------------------------------

using LeVent.Clients;
using MyProject.Models.Services.Foundations.PostEvents;

namespace MyProject.Brokers.Events
{
    // events-011: EventBroker is a partial class
    public partial class EventBroker : IEventBroker
    {
        public EventBroker()
        {
            // events-014: Each entity gets its own LeVentClient instance
            this.PostEvents = new LeVentClient<Post>();
        }
    }
}

// ---------------------------------------------------------------
// File: EventBroker_Post.cs
// events-012: Entity-specific implementation partial
// events-016: Thin pass-through to LeVentClient -- no logic
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using LeVent.Clients;
using MyProject.Models.Services.Foundations.PostEvents;

namespace MyProject.Brokers.Events
{
    public partial class EventBroker
    {
        // events-014: LeVentClient property for this entity
        public ILeVentClient<Post> PostEvents { get; set; }

        // events-016: Broker is a thin pass-through -- no business logic
        public ValueTask PublishPostEventAsync(Post post, string eventName = null) =>
            this.PostEvents.PublishEventAsync(post, eventName);

        public void SubscribeToPostEvent(
            Func<Post, ValueTask> postEventHandler, string eventName = null) =>
                this.PostEvents.RegisterEventHandler(postEventHandler, eventName);
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.cs (root)
// events-053: Root contains only mocks, service, and helpers
// events-074: No loggingBrokerMock declared
// ---------------------------------------------------------------

using System;
using Moq;
using MyProject.Brokers.Events;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Services.Foundations.PostEvents;
using Tynamix.ObjectFiller;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        // events-074: Only eventBrokerMock -- no loggingBrokerMock
        private readonly Mock<IEventBroker> eventBrokerMock;
        private readonly IPostEventService postEventService;

        public PostEventServiceTests()
        {
            this.eventBrokerMock = new Mock<IEventBroker>();

            // events-023: Service only receives the event broker
            this.postEventService = new PostEventService(
                eventBroker: this.eventBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static Post CreateRandomPost() =>
            CreatePostFiller(GetRandomDateTimeOffset()).Create();

        private static Filler<Post> CreatePostFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<Post>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(post => post.Title).Use(GetRandomString())
                .OnProperty(post => post.Content).Use(GetRandomString());

            return filler;
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Logic.Subscribe.cs
// events-070: Subscribe happy path tested first
// events-075: Handler mock is Mock<Func<Post, ValueTask>>
// events-076: VerifyNoOtherCalls at the end
// events-077: Times.Once for expected call
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public void ShouldSubscribeToPostEvents()
        {
            // given
            // events-075: Handler mock typed as Mock<Func<[Entity], ValueTask>>
            var postEventHandlerMock = new Mock<Func<Post, ValueTask>>();

            // when
            // events-063: Method is SubscribeToPostEvent -- not ListenToPostEvent
            this.postEventService.SubscribeToPostEvent(postEventHandlerMock.Object);

            // then
            // events-077: Times.Once for the expected broker call
            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeToPostEvent(postEventHandlerMock.Object),
                    Times.Once);

            // events-076: Always VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Validations.Subscribe.cs
// events-072: Null handler validation failure tested
// events-077: Times.Never -- broker must not be called
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Models.Foundations.PostEvents.Exceptions;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnSubscribeToPostEventIfEventHandlerIsNull()
        {
            // given
            Func<Post, ValueTask> postEventHandlerMock = null;

            var nullPostEventHandlerException =
                new NullPostEventHandlerException(message: "Post event handler is null.");

            var expectedPostEventValidationException = new PostEventValidationException(
                message: "Post event validation error occurred, please try again.",
                innerException: nullPostEventHandlerException);

            // when
            Action subscribeToPostEventAction = () =>
                this.postEventService.SubscribeToPostEvent(postEventHandlerMock);

            PostEventValidationException actualPostEventValidationException =
                Assert.Throws<PostEventValidationException>(subscribeToPostEventAction);

            // then
            actualPostEventValidationException.Should()
                .BeEquivalentTo(expectedPostEventValidationException);

            // events-077: Times.Never -- broker must not be called when validation fails
            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeToPostEvent(postEventHandlerMock),
                    Times.Never);

            // events-076: VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Exceptions.Subscribe.cs
// events-073: Service exception tested for subscribe
// events-076: VerifyNoOtherCalls at the end
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Models.Foundations.PostEvents.Exceptions;
using Xeptions;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public void ShouldThrowServiceExceptionOnSubscribeToPostEventIfServiceErrorOccurs()
        {
            // given
            var postEventHandlerMock = new Mock<Func<Post, ValueTask>>();
            var serviceException = new Exception();

            var failedPostServiceException = new FailedPostEventServiceException(
                message: "Failed post event service error occurred, please contact support",
                innerException: serviceException as Xeption,
                data: serviceException.Data);

            var expectedPostEventServiceException = new PostEventServiceException(
                message: "Post event service error occurred, please contact support",
                innerException: failedPostServiceException);

            this.eventBrokerMock.Setup(broker =>
                broker.SubscribeToPostEvent(postEventHandlerMock.Object))
                    .Throws(serviceException);

            // when
            Action subscribeToPostEventAction = () =>
                this.postEventService.SubscribeToPostEvent(postEventHandlerMock.Object);

            PostEventServiceException actualPostEventServiceException =
                Assert.Throws<PostEventServiceException>(subscribeToPostEventAction);

            // then
            actualPostEventServiceException.Should()
                .BeEquivalentTo(expectedPostEventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeToPostEvent(postEventHandlerMock.Object),
                    Times.Once);

            // events-076: VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
