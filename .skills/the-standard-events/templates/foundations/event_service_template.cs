// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Foundation Event Service — All three axes
// Replace [Entity] / [entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Main logic, Validations, and Exceptions files.

// ---------------------------------------------------------------
// File: I[Entity]EventService.cs
// Interface defining event service contract
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Models.Services.Foundations.[Entity]Events;

namespace [Namespace].Services.Foundations.[Entity]Events
{
    internal interface I[Entity]EventService
    {
        void SubscribeTo[Entity]Event(Func<[Entity], ValueTask> [entity]EventHandler);
        ValueTask Publish[Entity]Async([Entity] [entity]);
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventService.cs
// Main service implementation with TryCatch wrappers
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Brokers.Events;
using [Namespace].Models.Services.Foundations.[Entity]Events;

namespace [Namespace].Services.Foundations.[Entity]Events
{
    internal partial class [Entity]EventService : I[Entity]EventService
    {
        private readonly IEventBroker eventBroker;

        public [Entity]EventService(IEventBroker eventBroker) =>
            this.eventBroker = eventBroker;

        public void SubscribeTo[Entity]Event(
            Func<[Entity], ValueTask> [entity]EventHandler) =>
                TryCatch(() =>
                {
                    Validate[Entity]EventHandler([entity]EventHandler);
                    this.eventBroker.SubscribeTo[Entity]Event([entity]EventHandler);
                });

        public ValueTask Publish[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnPublish([entity]);
                await this.eventBroker.Publish[Entity]EventAsync([entity]);
            });
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventService.Validations.cs
// Validation logic for event service operations
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using [Namespace].Models.Foundations.[Entity]Events.Exceptions;

namespace [Namespace].Services.Foundations.[Entity]Events
{
    internal partial class [Entity]EventService
    {
        private void Validate[Entity]EventHandler(
            Func<[Entity], ValueTask> [entity]EventHandler)
        {
            Validate[Entity]EventHandlerIsNotNull([entity]EventHandler);
        }

        private void Validate[Entity]OnPublish([Entity] [entity])
        {
            Validate[Entity]IsNotNull([entity]);
        }

        private static void Validate[Entity]EventHandlerIsNotNull(
            Func<[Entity], ValueTask> [entity]EventHandler)
        {
            if ([entity]EventHandler is null)
            {
                throw new Null[Entity]EventHandlerException(message: "[Entity] event handler is null.");
            }
        }

        private static void Validate[Entity]IsNotNull([Entity] [entity])
        {
            if ([entity] is null)
            {
                throw new Null[Entity]EventException(message: "[Entity] status event handler is null.");
            }
        }
    }
}

// ---------------------------------------------------------------
// File: [Entity]EventService.Exceptions.cs
// Exception handling and mapping logic
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Models.Services.Foundations.[Entity]Events.Exceptions;
using Xeptions;

namespace [Namespace].Services.Foundations.[Entity]Events
{
    internal partial class [Entity]EventService
    {
        private delegate void ReturningNothingFunction();
        private delegate ValueTask ReturningValueTaskFunction();

        private void TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                returningNothingFunction();
            }
            catch (Null[Entity]EventHandlerException null[Entity]EventHandler)
            {
                throw CreateAndLogValidationException(null[Entity]EventHandler);
            }
            catch (Exception exception)
            {
                var failed[Entity]EventServiceException = new Failed[Entity]EventServiceException(
                    message: "Failed [entity] event service error occurred, please contact support",
                    innerException: exception as Xeption,
                    data: exception.Data);

                throw CreateAndLogServiceException(failed[Entity]EventServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningValueTaskFunction returningValueTaskFunction)
        {
            try
            {
                await returningValueTaskFunction();
            }
            catch (Null[Entity]EventException null[Entity]EventException)
            {
                throw CreateAndLogValidationException(null[Entity]EventException);
            }
            catch (Exception exception)
            {
                var failed[Entity]EventServiceException = new Failed[Entity]EventServiceException(
                    message: "Failed [entity] event service error occurred, please contact support",
                    innerException: exception as Xeption,
                    data: exception.Data);

                throw CreateAndLogServiceException(failed[Entity]EventServiceException);
            }
        }

        private [Entity]EventValidationException CreateAndLogValidationException(Xeption exception)
        {
            var [entity]EventValidationException = new [Entity]EventValidationException(
                message: "[Entity] event validation error occurred, please try again.",
                innerException: exception);

            return [entity]EventValidationException;
        }

        private [Entity]EventServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var [entity]EventServiceException = new [Entity]EventServiceException(
                message: "[Entity] event service error occurred, please contact support",
                innerException: exception);

            return [entity]EventServiceException;
        }
    }
}
