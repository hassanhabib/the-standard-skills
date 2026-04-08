// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Service Tests -- Subscribe Operation (single-file reference)
// Replace [Entity] / [entity] / [Namespace] with actual values.
// For the full multi-file template see: event_service_test_class_subscribe_template.cs

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Logic.Subscribe.cs
// events-070: Happy-path test for SubscribeTo[Entity]Event runs first
// events-075: Handler mock typed as Mock<Func<[Entity], ValueTask>>
// events-076: VerifyNoOtherCalls on eventBrokerMock
// events-077: Times.Once for expected call
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Moq;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entity]Events
{
    public partial class [Entity]EventServiceTests
    {
        [Fact]
        public void ShouldSubscribeTo[Entity]Events()
        {
            // given
            // events-075: Handler is Mock<Func<[Entity], ValueTask>>
            var [entity]EventHandlerMock = new Mock<Func<[Entity], ValueTask>>();

            // when
            // events-063: SubscribeTo -- never ListenTo
            this.[entity]EventService.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object);

            // then
            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Validations.Subscribe.cs
// events-072: Null handler validation failure test
// events-077: Times.Never -- broker must not be called
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using [Namespace].Models.Foundations.[Entity]Events.Exceptions;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entity]Events
{
    public partial class [Entity]EventServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnSubscribeTo[Entity]EventIfEventHandlerIsNull()
        {
            // given
            Func<[Entity], ValueTask> [entity]EventHandlerMock = null;

            var null[Entity]EventHandlerException =
                new Null[Entity]EventHandlerException(message: "[Entity] event handler is null.");

            var expected[Entity]EventValidationException = new [Entity]EventValidationException(
                message: "[Entity] event validation error occurred, please try again.",
                innerException: null[Entity]EventHandlerException);

            // when
            Action subscribeTo[Entity]EventAction = () =>
                this.[entity]EventService.SubscribeTo[Entity]Event([entity]EventHandlerMock);

            [Entity]EventValidationException actual[Entity]EventValidationException =
                Assert.Throws<[Entity]EventValidationException>(subscribeTo[Entity]EventAction);

            // then
            actual[Entity]EventValidationException.Should()
                .BeEquivalentTo(expected[Entity]EventValidationException);

            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeTo[Entity]Event([entity]EventHandlerMock),
                    Times.Never);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Exceptions.Subscribe.cs
// events-073: Service exception test for Subscribe
// events-076: VerifyNoOtherCalls at the end
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using [Namespace].Models.Foundations.[Entity]Events.Exceptions;
using Xeptions;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entity]Events
{
    public partial class [Entity]EventServiceTests
    {
        [Fact]
        public void ShouldThrowServiceExceptionOnSubscribeTo[Entity]EventIfServiceErrorOccurs()
        {
            // given
            var [entity]EventHandlerMock = new Mock<Func<[Entity], ValueTask>>();
            var serviceException = new Exception();

            var failed[Entity]ServiceException = new Failed[Entity]EventServiceException(
                message: "Failed [entity] event service error occurred, please contact support",
                innerException: serviceException as Xeption,
                data: serviceException.Data);

            var expected[Entity]EventServiceException = new [Entity]EventServiceException(
                message: "[Entity] event service error occurred, please contact support",
                innerException: failed[Entity]ServiceException);

            this.eventBrokerMock.Setup(broker =>
                broker.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object))
                    .Throws(serviceException);

            // when
            Action subscribeTo[Entity]EventAction = () =>
                this.[entity]EventService.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object);

            [Entity]EventServiceException actual[Entity]EventServiceException =
                Assert.Throws<[Entity]EventServiceException>(subscribeTo[Entity]EventAction);

            // then
            actual[Entity]EventServiceException.Should()
                .BeEquivalentTo(expected[Entity]EventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
