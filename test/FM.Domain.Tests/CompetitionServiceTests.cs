using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using FM.Domain.Services;

namespace FM.Domain.Tests
{
    public class CompetitionServiceTests
    {
        private CompetitionService _target;

        public CompetitionServiceTests()
        {
            var loggerMock = new Mock<ILogger<CompetitionService>>();
            _target = new CompetitionService(loggerMock.Object);
        }

        [Fact]
        public void CreateCompetition_ReturnsNewCompetition()
        {
            var expectedName = "Foo";
            var actual = _target.CreateCompetition(expectedName);

            Assert.NotNull(actual);
            Assert.Equal(expectedName, actual.Name);
        }

        [Fact]
        public void CreateCompetition_CreatesRankingForEachTeam()
        {
            var actual = _target.CreateCompetition("Foo");

            Assert.Equal(actual.Rankings.Count, actual.Teams.Count);
        }
    }
}
