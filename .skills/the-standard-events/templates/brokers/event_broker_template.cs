// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Event Broker — Infrastructure-Agnostic Implementation
// Replace [Entity] / [entity] / [Entities] / [Namespace] with actual values.
// Replace [EventClient] / [IEventClient] with your chosen event infrastructure client type.
// Demonstrates: Interface and implementation for event-driven architecture.

// NOTE: This template is infrastructure-agnostic. You may use any event library that fits your needs.
//       Examples: LeVent, EventHighway, Azure Service Bus, RabbitMQ, Kafka, MassTransit, etc.
//       The example below uses LeVent, but you can substitute any event infrastructure.

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

// EXAMPLE: Using LeVent as the event infrastructure
// Replace with your chosen event infrastructure library:
using LeVent.Clients;
// using EventHighway;
// using Azure.Messaging.ServiceBus;
// using RabbitMQ.Client;
// using Confluent.Kafka;

namespace [Namespace].Brokers.Events
{
    public partial class EventBroker : IEventBroker
    {
        public EventBroker()
        {
            // EXAMPLE: Initialize LeVent client for this entity
            // Replace with your chosen event infrastructure initialization:
            this.[Entity]Events = new LeVentClient<[Entity]>();

            // ALTERNATIVE EXAMPLES (commented out):
            // this.[Entity]Events = new EventHighwayClient<[Entity]>();
            // this.[Entity]Events = new ServiceBusClient(connectionString).CreateSender(queueName);
            // this.[Entity]Events = new ConnectionFactory().CreateConnection().CreateModel();
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

// EXAMPLE: Using LeVent as the event infrastructure
// Replace with your chosen event infrastructure library:
using LeVent.Clients;
// using EventHighway;
// using Azure.Messaging.ServiceBus;
// using RabbitMQ.Client;

namespace [Namespace].Brokers.Events
{
    public partial class EventBroker
    {
        // EXAMPLE: LeVent client property
        // Replace ILeVentClient<[Entity]> and [EventClient] with your infrastructure's client type:
        public ILeVentClient<[Entity]> [Entity]Events { get; set; }

        // ALTERNATIVE EXAMPLES (commented out):
        // public IEventHighwayClient<[Entity]> [Entity]Events { get; set; }
        // public ServiceBusSender [Entity]Events { get; set; }
        // public IModel [Entity]Events { get; set; }

        public ValueTask Publish[Entity]Async([Entity] [entity], string eventName = null) =>
            this.[Entity]Events.PublishEventAsync([entity], eventName);

        // ALTERNATIVE EXAMPLES (commented out):
        // this.[Entity]Events.PublishAsync([entity], eventName);
        // this.[Entity]Events.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize([entity])));
        // this.[Entity]Events.BasicPublish(
        //      exchange: "",
        //      routingKey: eventName,
        //      body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize([entity])));

        public void SubscribeTo[Entity]Event(Func<[Entity], ValueTask> [entity]EventHandler, string eventName = null) =>
            this.[Entity]Events.RegisterEventHandler([entity]EventHandler, eventName);

        // ALTERNATIVE EXAMPLES (commented out):
        // this.[Entity]Events.Subscribe([entity]EventHandler, eventName);
        // this.[Entity]Events.OnMessageAsync(async (message) =>
        //      await [entity]EventHandler(JsonSerializer.Deserialize<[Entity]>(message.Body)));

        // var consumer = new EventingBasicConsumer(channel); consumer.Received += async (model, ea) =>
        // {
        //      var [entity] = JsonSerializer.Deserialize<[Entity]>(ea.Body.ToArray());
        //      await [entity]EventHandler([entity]);
        // };
    }
}
