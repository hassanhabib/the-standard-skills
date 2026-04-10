// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License")
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;
using Moq;
using [Namespace].Brokers.Events;
using [Namespace].Models.Services.Foundations.[Entity]Events;
using [Namespace].Services.Foundations.[Entity]Events;
using Tynamix.ObjectFiller;

namespace [Namespace].Tests.Unit.Services.Foundations.[Entity]Events
{
    public partial class [Entity]
EventServiceTests
    {
        private readonly Mock<IEventBroker> eventBrokerMock;
        private readonly I[Entity]EventService [entity]EventService;

        public [Entity]EventServiceTests()
        {
            this.eventBrokerMock = new Mock<IEventBroker>();

            this.[entity]EventService = new [Entity]EventService(
                eventBroker: this.eventBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static [Entity] CreateRandom[Entity](DateTimeOffset? dateTimeOffset = null) =>
            Create[Entity]Filler(dateTimeOffset ?? GetRandomDateTimeOffset()).Create();

        private static Filler<[Entity]> Create[Entity]Filler(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<[Entity]>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(borough => borough.Message).Use(GetRandomString())
                .OnProperty(borough => borough.Status).Use(GetRandomString())
                .OnProperty(borough => borough.[Entity]Items).Use(GetRandomNumber())
                .OnProperty(borough => borough.TotalItems).Use(GetRandomNumber());

            return filler;
        }
    }
}
