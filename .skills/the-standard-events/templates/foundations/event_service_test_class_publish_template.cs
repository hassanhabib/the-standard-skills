// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Service Tests — Publish Operation
// Replace [Entity] / [entity] / [Namespace] with actual values.
// Demonstrates: Logic, Validations, and Exceptions for Publish operations.

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Logic.Publish.cs
// Happy-path tests for publish operation
// ---------------------------------------------------------------

using System.Threading.Tasks;
using Moq;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using Xunit;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entity]Events
{
    public partial class [Entity]EventServiceTests
    {
        [Fact]
        public async Task ShouldPublish[Entity]Async()
        {
            // given
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = random[Entity];

            // when
            await this.[entity]EventService
                .Publish[Entity]Async(input[Entity]);

            // then
            this.eventBrokerMock.Verify(broker =>
                broker.Publish[Entity]EventAsync(input[Entity]),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Validations.Publish.cs
// Validation failure tests for publish operation
// ---------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnPublishIf[Entity]IsNullAsync()
        {
            // given
            [Entity] null[Entity] = null;

            var null[Entity]EventException = new Null[Entity]EventException(
                message: "[Entity] status event handler is null.");

            var expected[Entity]EventValidationException = new [Entity]EventValidationException(
                message: "[Entity] event validation error occurred, please try again.",
                innerException: null[Entity]EventException);

            // when
            ValueTask publish[Entity]Task =
                this.[entity]EventService.Publish[Entity]Async(null[Entity]);

            [Entity]EventValidationException actual[Entity]EventValidationException =
                await Assert.ThrowsAsync<[Entity]EventValidationException>(publish[Entity]Task.AsTask);

            // then
            actual[Entity]EventValidationException.Should()
                .BeEquivalentTo(expected[Entity]EventValidationException);

            this.eventBrokerMock.Verify(broker =>
                broker.Publish[Entity]EventAsync(null[Entity]),
                    Times.Never);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Exceptions.Publish.cs
// Exception tests for publish operation
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
        public async Task ShouldThrowServiceExceptionOnPublish[Entity]EventIfServiceErrorOccursAsync()
        {
            // given
            [Entity] random[Entity] = CreateRandom[Entity]();
            [Entity] input[Entity] = random[Entity];

            var serviceException = new Exception();

            var failed[Entity]EventServiceException = new Failed[Entity]EventServiceException(
                message: "Failed [entity] event service error occurred, please contact support",
                innerException: serviceException as Xeption,
                data: serviceException.Data);

            var expected[Entity]EventServiceException = new [Entity]EventServiceException(
                message: "[Entity] event service error occurred, please contact support",
                innerException: failed[Entity]EventServiceException);

            this.eventBrokerMock.Setup(broker =>
                broker.Publish[Entity]EventAsync(input[Entity]))
                    .Throws(serviceException);

            // when
            ValueTask subscribeTo[Entity]EventTask = this.[entity]EventService
                .Publish[Entity]Async(input[Entity]);

            [Entity]EventServiceException actual[Entity]EventValidationException =
                await Assert.ThrowsAsync<[Entity]EventServiceException>(
                    subscribeTo[Entity]EventTask.AsTask);

            // then
            actual[Entity]EventValidationException.Should()
                .BeEquivalentTo(expected[Entity]EventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.Publish[Entity]EventAsync(input[Entity]),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
