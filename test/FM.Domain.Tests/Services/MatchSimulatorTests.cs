using FM.Domain.Models;
using FM.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace FM.Domain.Tests.Services
{
    public class MatchSimulatorTests
    {
        private readonly MatchSimulator _target;

        public MatchSimulatorTests()
        {
            var loggerMock = new Mock<ILogger<MatchSimulator>>();
            var matchEventServiceMock = new Mock<IMatchEventService>();
            matchEventServiceMock.Setup(x => x.GetEvents()).Returns(new List<Event> { new Event("foo", EventTypes.Other, 1) } );
            _target = new MatchSimulator(loggerMock.Object, matchEventServiceMock.Object);
        }

        [Fact]
        public void Simulate_UpdatesHomeAndAwayGoals()
        {
            var match = new Domain.Models.Match { HomeTeam = new Team("Foo", 10, 10), AwayTeam = new Team("010", 20, 10) };

            var matchPlayed =_target.Simulate(match);

            Assert.True(matchPlayed.HomeGoals.HasValue);
            Assert.True(matchPlayed.AwayGoals.HasValue);
        }
    }
}
