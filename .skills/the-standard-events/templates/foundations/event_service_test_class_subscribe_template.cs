// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Service Tests — Subscribe Operation
// Replace [Entity] / [entity] / [Namespace] with actual values.
// Demonstrates: Logic, Validations, and Exceptions for Subscribe operations.

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Logic.Subscribe.cs
// Happy-path tests for subscribe operation
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
            var [entity]EventHandlerMock =
                new Mock<Func<[Entity], ValueTask>>();

            // when
            this.[entity]EventService.SubscribeTo[Entity]Event(
                [entity]EventHandlerMock.Object);

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
// Validation failure tests for subscribe operation
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

            var null[Entity]EventHandler =
                new Null[Entity]EventHandlerException(message: "[Entity] event handler is null.");

            var expected[Entity]EventValidationException = new [Entity]EventValidationException(
                message: "[Entity] event validation error occurred, please try again.",
                innerException: null[Entity]EventHandler);

            // when
            Action subscribeTo[Entity]EventAction = () => this.[entity]EventService
                .SubscribeTo[Entity]Event([entity]EventHandlerMock);

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
// Exception tests for subscribe operation
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
            var [entity]EventHandlerMock =
                new Mock<Func<[Entity], ValueTask>>();

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
            Action subscribeTo[Entity]EventAction = () => this.[entity]EventService
                .SubscribeTo[Entity]Event([entity]EventHandlerMock.Object);

            [Entity]EventServiceException actual[Entity]EventValidationException =
                Assert.Throws<[Entity]EventServiceException>(subscribeTo[Entity]EventAction);

            // then
            actual[Entity]EventValidationException.Should()
                .BeEquivalentTo(expected[Entity]EventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.SubscribeTo[Entity]Event([entity]EventHandlerMock.Object),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
