// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// GOOD EXAMPLE: Standard-compliant Event Service -- Publish Operation
// Demonstrates: service interface, implementation, validation, exceptions, and tests.

// ---------------------------------------------------------------
// File: IPostEventService.cs
// events-060: Interface named I[Entity]EventService
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.PostEvents;

namespace MyProject.Services.Foundations.PostEvents
{
    // events-060: I[Entity]EventService interface contract
    // events-020: Exactly two operations per entity
    internal interface IPostEventService
    {
        void SubscribeToPostEvent(Func<Post, ValueTask> postEventHandler);
        ValueTask PublishPostAsync(Post post);
    }
}

// ---------------------------------------------------------------
// File: PostEventService.cs
// events-061: Implementation named [Entity]EventService
// events-024: internal partial class
// events-003: Only IEventBroker injected -- no other dependency
// events-023: No ILoggingBroker injected
// events-025: TryCatch delegates used
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Brokers.Events;
using MyProject.Models.Services.Foundations.PostEvents;

namespace MyProject.Services.Foundations.PostEvents
{
    internal partial class PostEventService : IPostEventService
    {
        private readonly IEventBroker eventBroker;

        // events-023: Constructor takes only IEventBroker
        public PostEventService(IEventBroker eventBroker) =>
            this.eventBroker = eventBroker;

        // events-063: SubscribeTo prefix -- not ListenTo
        // events-022: Synchronous (void)
        public void SubscribeToPostEvent(
            Func<Post, ValueTask> postEventHandler) =>
                TryCatch(() =>
                {
                    ValidatePostEventHandler(postEventHandler);
                    this.eventBroker.SubscribeToPostEvent(postEventHandler);
                });

        // events-062: Publish[Entity]Async naming
        // events-021: Async (ValueTask)
        public ValueTask PublishPostAsync(Post post) =>
            TryCatch(async () =>
            {
                ValidatePostOnPublish(post);
                await this.eventBroker.PublishPostEventAsync(post);
            });
    }
}

// ---------------------------------------------------------------
// File: PostEventService.Validations.cs
// events-030: Publish validates entity not null
// events-031: Subscribe validates handler not null
// events-032: Null entity throws NullPostEventException
// events-033: Null handler throws NullPostEventHandlerException
// events-034: Circuit-breaking -- throw immediately, no collection
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Models.Foundations.PostEvents.Exceptions;

namespace MyProject.Services.Foundations.PostEvents
{
    internal partial class PostEventService
    {
        private void ValidatePostEventHandler(
            Func<Post, ValueTask> postEventHandler)
        {
            ValidatePostEventHandlerIsNotNull(postEventHandler);
        }

        private void ValidatePostOnPublish(Post post)
        {
            ValidatePostIsNotNull(post);
        }

        private static void ValidatePostEventHandlerIsNotNull(
            Func<Post, ValueTask> postEventHandler)
        {
            if (postEventHandler is null)
            {
                // events-033: NullEventHandlerException for null handler
                throw new NullPostEventHandlerException(
                    message: "Post event handler is null.");
            }
        }

        private static void ValidatePostIsNotNull(Post post)
        {
            if (post is null)
            {
                // events-032: NullEventException for null entity
                throw new NullPostEventException(
                    message: "Post status event handler is null.");
            }
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventService.Exceptions.cs
// events-040: Two TryCatch delegates (void and ValueTask)
// events-041: Unexpected exceptions -> FailedPostEventServiceException
// events-042: Wrapped in PostEventServiceException
// events-043: No Dependency or CriticalDependency exception categories
// events-044: FailedEventServiceException carries innerException and Data
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyProject.Models.Services.Foundations.PostEvents.Exceptions;
using Xeptions;

namespace MyProject.Services.Foundations.PostEvents
{
    internal partial class PostEventService
    {
        // events-040: Sync delegate for SubscribeTo operations
        private delegate void ReturningNothingFunction();
        // events-040: Async delegate for Publish operations
        private delegate ValueTask ReturningValueTaskFunction();

        private void TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                returningNothingFunction();
            }
            catch (NullPostEventHandlerException nullPostEventHandlerException)
            {
                // events-035: Wrap in [Entity]EventValidationException
                throw CreateAndLogValidationException(nullPostEventHandlerException);
            }
            catch (Exception exception)
            {
                // events-041, events-044: Wrap in FailedEventServiceException with innerException + Data
                var failedPostEventServiceException = new FailedPostEventServiceException(
                    message: "Failed post event service error occurred, please contact support",
                    innerException: exception as Xeption,
                    data: exception.Data);

                // events-042: Wrap in [Entity]EventServiceException
                throw CreateAndLogServiceException(failedPostEventServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningValueTaskFunction returningValueTaskFunction)
        {
            try
            {
                await returningValueTaskFunction();
            }
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
        }

        private PostEventValidationException CreateAndLogValidationException(Xeption exception)
        {
            var postEventValidationException = new PostEventValidationException(
                message: "Post event validation error occurred, please try again.",
                innerException: exception);

            return postEventValidationException;
        }

        private PostEventServiceException CreateAndLogServiceException(Xeption exception)
        {
            var postEventServiceException = new PostEventServiceException(
                message: "Post event service error occurred, please contact support",
                innerException: exception);

            return postEventServiceException;
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Logic.Publish.cs
// events-071: Publish happy path tested
// events-076: VerifyNoOtherCalls at the end
// events-077: Times.Once for expected call
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public async Task ShouldPublishPostAsync()
        {
            // given
            Post randomPost = CreateRandomPost();
            Post inputPost = randomPost;

            // when
            await this.postEventService.PublishPostAsync(inputPost);

            // then
            // events-077: Times.Once for expected call
            this.eventBrokerMock.Verify(broker =>
                broker.PublishPostEventAsync(inputPost),
                    Times.Once);

            // events-076: Always VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Validations.Publish.cs
// events-072: Validation failure tested
// events-077: Times.Never for skipped broker call
// ---------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Models.Services.Foundations.PostEvents.Exceptions;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPublishIfPostIsNullAsync()
        {
            // given
            Post nullPost = null;

            var nullPostEventException = new NullPostEventException(
                message: "Post status event handler is null.");

            var expectedPostEventValidationException = new PostEventValidationException(
                message: "Post event validation error occurred, please try again.",
                innerException: nullPostEventException);

            // when
            ValueTask publishPostTask = this.postEventService.PublishPostAsync(nullPost);

            PostEventValidationException actualPostEventValidationException =
                await Assert.ThrowsAsync<PostEventValidationException>(publishPostTask.AsTask);

            // then
            actualPostEventValidationException.Should()
                .BeEquivalentTo(expectedPostEventValidationException);

            // events-077: Times.Never -- broker must not be called when validation fails
            this.eventBrokerMock.Verify(broker =>
                broker.PublishPostEventAsync(nullPost),
                    Times.Never);

            // events-076: VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: PostEventServiceTests.Exceptions.Publish.cs
// events-073: Service exception tested
// events-076: VerifyNoOtherCalls at the end
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyProject.Models.Services.Foundations.PostEvents;
using MyProject.Models.Services.Foundations.PostEvents.Exceptions;
using Xeptions;
using Xunit;

namespace MyProject.Tests.Unit.Services.Foundations.PostEvents
{
    public partial class PostEventServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnPublishPostEventIfServiceErrorOccursAsync()
        {
            // given
            Post randomPost = CreateRandomPost();
            Post inputPost = randomPost;
            var serviceException = new Exception();

            var failedPostEventServiceException = new FailedPostEventServiceException(
                message: "Failed post event service error occurred, please contact support",
                innerException: serviceException as Xeption,
                data: serviceException.Data);

            var expectedPostEventServiceException = new PostEventServiceException(
                message: "Post event service error occurred, please contact support",
                innerException: failedPostEventServiceException);

            this.eventBrokerMock.Setup(broker =>
                broker.PublishPostEventAsync(inputPost))
                    .Throws(serviceException);

            // when
            ValueTask publishPostTask = this.postEventService.PublishPostAsync(inputPost);

            PostEventServiceException actualPostEventServiceException =
                await Assert.ThrowsAsync<PostEventServiceException>(publishPostTask.AsTask);

            // then
            actualPostEventServiceException.Should()
                .BeEquivalentTo(expectedPostEventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.PublishPostEventAsync(inputPost),
                    Times.Once);

            // events-076: VerifyNoOtherCalls
            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
