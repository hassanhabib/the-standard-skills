// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License")
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using Moq;
using Standardly.Core.Brokers.Events;
using Standardly.Core.Models.Services.Foundations.ProcessedEvents;
using Standardly.Core.Services.Foundations.ProcessedEvents;
using Tynamix.ObjectFiller;

namespace Standardly.Core.Tests.Unit.Services.Foundations.ProcessedEvents
{
    public partial class ProcessedEventServiceTests
    {
        private readonly Mock<IEventBroker> eventBrokerMock;
        private readonly IProcessedEventService processedEventService;

        public ProcessedEventServiceTests()
        {
            this.eventBrokerMock = new Mock<IEventBroker>();

            this.processedEventService = new ProcessedEventService(
                eventBroker: this.eventBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static Processed CreateRandomProcessed(DateTimeOffset? dateTimeOffset = null) =>
            CreateProcessedFiller(dateTimeOffset ?? GetRandomDateTimeOffset()).Create();

        private static Filler<Processed> CreateProcessedFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<Processed>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(borough => borough.Message).Use(GetRandomString())
                .OnProperty(borough => borough.Status).Use(GetRandomString())
                .OnProperty(borough => borough.ProcessedItems).Use(GetRandomNumber())
                .OnProperty(borough => borough.TotalItems).Use(GetRandomNumber());

            return filler;
        }
    }
}
