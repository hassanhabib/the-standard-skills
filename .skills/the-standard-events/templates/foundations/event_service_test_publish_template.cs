// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Service Tests -- Publish Operation (single-file reference)
// Replace [Entity] / [entity] / [Namespace] with actual values.
// For the full multi-file template see: event_service_test_class_publish_template.cs

// ---------------------------------------------------------------
// File: [Entity]EventServiceTests.Logic.Publish.cs
// events-071: Happy-path test for Publish[Entity]Async
// events-076: VerifyNoOtherCalls on eventBrokerMock
// events-077: Times.Once for expected call
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
            await this.[entity]EventService.Publish[Entity]Async(input[Entity]);

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
// events-072: Null entity validation failure test
// events-077: Times.Never -- broker must not be called
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
// events-073: Service exception test for Publish
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
            ValueTask publish[Entity]Task =
                this.[entity]EventService.Publish[Entity]Async(input[Entity]);

            [Entity]EventServiceException actual[Entity]EventServiceException =
                await Assert.ThrowsAsync<[Entity]EventServiceException>(publish[Entity]Task.AsTask);

            // then
            actual[Entity]EventServiceException.Should()
                .BeEquivalentTo(expected[Entity]EventServiceException);

            this.eventBrokerMock.Verify(broker =>
                broker.Publish[Entity]EventAsync(input[Entity]),
                    Times.Once);

            this.eventBrokerMock.VerifyNoOtherCalls();
        }
    }
}
