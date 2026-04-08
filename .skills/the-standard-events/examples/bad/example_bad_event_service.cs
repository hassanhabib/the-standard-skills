// ---------------------------------------------------------------
// BAD EXAMPLE: Non-Standard Event Service Violations
// Each violation annotated with the rule it breaks.
// ---------------------------------------------------------------

namespace MyProject.Services.Foundations.PostEvents
{
    // events-024 VIOLATION: class is not internal
    // events-023 VIOLATION: ILoggingBroker injected into event service
    public partial class PostEventService : IPostEventService
    {
        private readonly IEventBroker eventBroker;
        private readonly ILoggingBroker loggingBroker; // events-023 VIOLATION

        public PostEventService(IEventBroker eventBroker, ILoggingBroker loggingBroker)
        {
            this.eventBroker = eventBroker;
            this.loggingBroker = loggingBroker; // events-023 VIOLATION
        }

        // events-063 VIOLATION: ListenTo prefix instead of SubscribeTo
        // events-022 VIOLATION: Missing TryCatch delegate
        public void ListenToPostEvent(Func<Post, ValueTask> postEventHandler)
        {
            // events-031 VIOLATION: No null check on handler before calling broker
            this.eventBroker.SubscribeToPostEvent(postEventHandler);
        }

        // events-021 VIOLATION: Method is not wrapped in TryCatch delegate
        public ValueTask PublishPostAsync(Post post)
        {
            // events-030 VIOLATION: No null check on entity before publishing
            return this.eventBroker.PublishPostEventAsync(post);
        }
    }
}

// ---------------------------------------------------------------
// events-016 VIOLATION: Business logic in the broker
// ---------------------------------------------------------------

namespace MyProject.Brokers.Events
{
    public partial class EventBroker
    {
        // events-016 VIOLATION: Conditional logic inside broker
        public ValueTask PublishPostEventAsync(Post post, string eventName = null)
        {
            if (post.IsPublished) // VIOLATION: logic in broker, not service
            {
                return this.PostEvents.PublishEventAsync(post, eventName);
            }

            return ValueTask.CompletedTask;
        }
    }
}

// ---------------------------------------------------------------
// events-043 VIOLATION: Dependency exception categories in event service
// ---------------------------------------------------------------

namespace MyProject.Services.Foundations.PostEvents
{
    internal partial class PostEventService
    {
        private void TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                returningNothingFunction();
            }
            catch (NullPostEventHandlerException nullPostEventHandlerException)
            {
                throw CreateAndLogValidationException(nullPostEventHandlerException);
            }
            // events-043 VIOLATION: Event services do not call HTTP APIs
            catch (HttpResponseNotFoundException httpResponseNotFoundException)
            {
                throw CreateAndLogCriticalDependencyException(httpResponseNotFoundException);
            }
            // events-043 VIOLATION: Event services do not call storage
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch (Exception exception)
            {
                var failedPostEventServiceException = new FailedPostEventServiceException(
                    message: "Failed post event service error occurred, please contact support",
                    innerException: exception as Xeption,
                    data: exception.Data);

                throw CreateAndLogServiceException(failedPostEventServiceException);
            }
        }
    }
}

// ---------------------------------------------------------------
// events-032 VIOLATION: Wrong exception type for null entity
// ---------------------------------------------------------------

namespace MyProject.Services.Foundations.PostEvents
{
    internal partial class PostEventService
    {
        private static void ValidatePostIsNotNull(Post post)
        {
            if (post is null)
            {
                // events-032 VIOLATION: Should be NullPostEventException
                throw new InvalidPostException(message: "Post is invalid.");
            }
        }
    }
}

// ---------------------------------------------------------------
// BAD TESTS: Event service test violations
// ---------------------------------------------------------------

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        private readonly Mock<IEventBroker> eventBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock; // events-074 VIOLATION
        private readonly IPostEventService postEventService;

        public PostEventServiceTests()
        {
            this.eventBrokerMock = new Mock<IEventBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>(); // events-074 VIOLATION

            this.postEventService = new PostEventService(
                eventBroker: this.eventBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object); // events-023 VIOLATION
        }

        [Fact]
        public async Task ShouldPublishPostAsync()
        {
            // given
            Post randomPost = CreateRandomPost();
            Post inputPost = randomPost;

            // when
            await this.postEventService.PublishPostAsync(inputPost);

            // then
            this.eventBrokerMock.Verify(broker =>
                broker.PublishPostEventAsync(inputPost),
                    Times.Once);

            // events-076 VIOLATION: Missing VerifyNoOtherCalls
        }

        // events-070 VIOLATION: Subscribe test not run before Publish test (order matters)
        // events-075 VIOLATION: Handler set as a raw Func, not Mock<Func<...>>
        [Fact]
        public void ShouldListenToPostEvents() // events-063 VIOLATION: ListenTo naming
        {
            // given
            Func<Post, ValueTask> postEventHandler = (post) => ValueTask.CompletedTask; // VIOLATION: not a Mock

            // when
            // events-063 VIOLATION: ListenTo instead of SubscribeTo
            this.postEventService.ListenToPostEvent(postEventHandler);

            // then
            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeToPostEvent(postEventHandler),
                    Times.Once);

            // events-076 VIOLATION: Missing VerifyNoOtherCalls
        }
    }
}
