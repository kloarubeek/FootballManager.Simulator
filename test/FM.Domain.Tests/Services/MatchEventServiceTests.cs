using FM.Domain.Models;
using FM.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FM.Domain.Tests.Services
{
    public class MatchEventServiceTests
    {
        private readonly MatchEventService _target;

        public MatchEventServiceTests()
        {
            var loggerMock = new Mock<ILogger<MatchEventService>>();

            _target = new MatchEventService(loggerMock.Object);
        }

        [Fact]
        public void CreateMatchEvent_GoalForHomeTeam_UpdatesMatchScore()
        {
            //arrange
            var homeTeam = new Team("North Abcoude", 1, 1) {TeamId = 1};
            var goalEvent = new Event("Goal", EventTypes.Strength, 10);
            var match = new Domain.Models.Match { HomeTeam = homeTeam, HomeGoals = 0, AwayGoals = 0 };

            //act
            _target.CreateMatchEvent(3, goalEvent, match, homeTeam);

            //assert
            Assert.Equal(1, match.HomeGoals);
            Assert.Equal(0, match.AwayGoals);
        }

        [Fact]
        public void CreateMatchEvent_ReturnsMatchEventWithMinuteAndTeam()
        {
            //arrange
            var expectedMinute = 34;
            var expectedTeam = new Team("VV Montfoort", 1, 1) { TeamId = 1 };
            var someEvent = new Event("MinuteOfSilence", EventTypes.Other, 1);

            //act
            var actual = _target.CreateMatchEvent(34, someEvent, new Domain.Models.Match(), expectedTeam);

            //assert
            Assert.Equal(expectedMinute, actual.Minute);
            Assert.Equal(expectedTeam, actual.Team);
        }

    }
}
