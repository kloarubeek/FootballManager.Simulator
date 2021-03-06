﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FM.Domain.Services;
using FM.Domain.Models;

namespace FM.Domain.Tests.Services
{
    public class StandingServiceTests
    {
        private StandingService _target;

        public StandingServiceTests()
        {
            var loggerMock = new Mock<ILogger<StandingService>>();
            _target = new StandingService(loggerMock.Object);
        }

        [Fact]
        public void UpdateStanding_SetsHomeAndAwayGoalsCorrect()
        {
            var ajax = new Team("Ajax", 1, 1) {TeamId = 1 };
            var feyenoord = new Team("Feyenoord", 1, 1) { TeamId = 2 };

            var competition = new Competition
            {
                Teams = new List<Team> { ajax, feyenoord }
            };

            foreach (var team in competition.Teams)
            {
                competition.Rankings.Add(new CompetitionRanking { Team = team });
            }

            var match = new Domain.Models.Match { HomeTeam = ajax, AwayTeam = feyenoord, HomeGoals = 4, AwayGoals = 3 };
            _target.UpdateStanding(competition, match);

            var ajaxRanking = competition.Rankings.Find(x => x.Team == ajax);
            var feyenoordRanking = competition.Rankings.Find(x => x.Team == feyenoord);

            Assert.Equal(4, ajaxRanking.GoalsFor);
            Assert.Equal(4, feyenoordRanking.GoalsAgainst);
            Assert.Equal(3, ajaxRanking.GoalsAgainst);
            Assert.Equal(3, feyenoordRanking.GoalsFor);
        }
    }
}
