// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Broker — LeVent Implementation
// Replace [Entity] / [entity] / [Entities] / [Namespace] with actual values.
// Demonstrates: Interface and implementation for event-driven architecture.

// ---------------------------------------------------------------
// File: IEventBroker.cs
// Base interface for event broker
// ---------------------------------------------------------------

namespace [Namespace].Brokers.Events
{
    public partial interface IEventBroker
    { }
}

// ---------------------------------------------------------------
// File: IEventBroker_[Entity].cs
// Entity-specific event operations interface
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Models.Foundations.[Entities];

namespace [Namespace].Brokers.Events
{
    public partial interface IEventBroker
    {
        ValueTask Publish[Entity]Async([Entity] [entity], string eventName = null);
        void SubscribeTo[Entity]Event(Func<[Entity], ValueTask> [entity]EventHandler, string eventName = null);
    }
}

// ---------------------------------------------------------------
// File: EventBroker.cs
// Base event broker implementation with constructor
// ---------------------------------------------------------------

using [Namespace].Models.Foundations.[Entities];

using LeVent.Clients;

namespace [Namespace].Brokers.Events
{
    public partial class EventBroker : IEventBroker
    {
        public EventBroker()
        {
            this.[Entity]Events = new LeVentClient<[Entity]>();
        }
    }
}

// ---------------------------------------------------------------
// File: EventBroker_[Entity].cs
// Entity-specific event operations implementation
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using [Namespace].Models.Foundations.[Entities];
using LeVent.Clients;

namespace [Namespace].Brokers.Events
{
    public partial class EventBroker
    {
        public ILeVentClient<[Entity]> [Entity]Events { get; set; }

        public ValueTask Publish[Entity]Async([Entity] [entity], string eventName = null) =>
            this.[Entity]Events.PublishEventAsync([entity], eventName);

        public void SubscribeTo[Entity]Event(Func<[Entity], ValueTask> [entity]EventHandler, string eventName = null) =>
            this.[Entity]Events.RegisterEventHandler([entity]EventHandler, eventName);
    }
}
