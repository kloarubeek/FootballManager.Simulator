using System.Collections.Generic;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using FM.Domain.Services;

namespace FM.Domain.Tests.Services
{
    public class CompetitionServiceTests
    {
        private readonly CompetitionService _target;
        private readonly Mock<ITeamService> _teamServiceMock;

        public CompetitionServiceTests()
        {
            var loggerMock = new Mock<ILogger<CompetitionService>>();
            var matchSimulatorMock = new Mock<IMatchSimulator>();
            var standingServiceMock = new Mock<IStandingService>();
            _teamServiceMock = new Mock<ITeamService>();

            _target = new CompetitionService(loggerMock.Object, _teamServiceMock.Object, matchSimulatorMock.Object, standingServiceMock.Object);
        }

        [Fact]
        public void CreateCompetition_ReturnsNewCompetition()
        {
            //assert
            _teamServiceMock.Setup(x => x.GetTeams()).Returns(new List<Team> { new Team("Foo", 1, 1) });
            var expectedName = "Foo";

            //act
            var actual = _target.CreateCompetition(expectedName);

            //assert
            Assert.NotNull(actual);
            Assert.Equal(expectedName, actual.Name);
        }

        [Fact]
        public void CreateCompetition_CreatesRankingForEachTeam()
        {
            //assert
            _teamServiceMock.Setup(x => x.GetTeams()).Returns(new List<Team> {new Team("010", 1, 1), new Team("020", 1, 1)});

            //act
            var actual = _target.CreateCompetition("Foo");

            //assert
            Assert.Equal(actual.Rankings.Count, actual.Teams.Count);
        }
    }
}
