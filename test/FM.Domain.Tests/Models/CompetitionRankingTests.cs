using System;
using System.Collections.Generic;
using System.Text;
using FM.Domain.Models;
using Xunit;

namespace FM.Domain.Tests.Models
{
    public class CompetitionRankingTests
    {
        [Fact]
        public void Equals_WithEqualRankings_ReturnsTrue()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1};
            var ranking2 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };

            Assert.True(ranking1.Equals(ranking2));
        }

        [Fact]
        public void Equals_WithNullRanking_ReturnsFalse()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };

            Assert.False(ranking1.Equals(null));
        }

        [Fact]
        public void Equals_WithDifferentPlayed_ReturnsFalse()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };
            var ranking2 = new CompetitionRanking { Played = 2, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };

            Assert.False(ranking1.Equals(ranking2));
        }

        [Fact]
        public void Equals_WithDifferentPoints_ReturnsFalse()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };
            var ranking2 = new CompetitionRanking { Played = 3, Points = 5, GoalsFor = 3, GoalsAgainst = 1 };

            Assert.False(ranking1.Equals(ranking2));
        }

        [Fact]
        public void Equals_WithDifferentGoalsFor_ReturnsFalse()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };
            var ranking2 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 4, GoalsAgainst = 1 };

            Assert.False(ranking1.Equals(ranking2));
        }

        [Fact]
        public void Equals_WithDifferentGoalDifference_ReturnsFalse()
        {
            var ranking1 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 1 };
            var ranking2 = new CompetitionRanking { Played = 3, Points = 4, GoalsFor = 3, GoalsAgainst = 2 };

            Assert.False(ranking1.Equals(ranking2));
        }

    }
}
