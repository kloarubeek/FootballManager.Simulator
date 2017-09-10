using System;
using System.Collections.Generic;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using Weighted_Randomizer;

namespace FM.Domain.Services
{
    public interface ICompetitionService
    {
        Competition CreateCompetition(string name);
        List<Match> CreateMatchSchedule(Competition competition);
    }

    public class CompetitionService : ICompetitionService
    {
        private readonly ILogger _logger;

        public CompetitionService(ILogger<CompetitionService> logger)
        {
            _logger = logger;
        }

        public Competition CreateCompetition(string name)
        {
            _logger.LogInformation($"Start competition {name}.");

            var competition = new Competition
            {
                Name = name,
                Teams = new List<Team>
                {
                    new Team { TeamId = 1, Name = "Ajax", Strength = 1000, Aggression = 10 },
                    new Team { TeamId = 2, Name = "Feyenoord", Strength = 100, Aggression = 40 },
                    new Team { TeamId = 3, Name = "PSV", Strength = 1, Aggression = 30 },
                    new Team { TeamId = 4, Name = "FC Utrecht", Strength = 100, Aggression = 20 }
                }
            };

            _logger.LogInformation("Create ranking.");
            foreach (var team in competition.Teams)
            {
                competition.Rankings.Add(new CompetitionRanking { Team = team });
            }
            return competition;
        }

        public List<Match> CreateMatchSchedule(Competition competition)
        {
            _logger.LogInformation("Create match schedule");

            var teams = competition.Teams.OrderBy(t => t.TeamId).ToList();
            var opponents = competition.Teams.OrderBy(t => t.TeamId).ToList();

            foreach (var team in teams)
            {
                opponents.Remove(team); //don't play against yourself
                foreach (var opponent in opponents)
                {
                    //randomizer to decide who's the home team
                    var matchTeams = new StaticWeightedRandomizer<Team> {team, opponent};
                    competition.Matches.Add(new Match { HomeTeam = matchTeams.NextWithRemoval(), AwayTeam = matchTeams.NextWithRemoval() });
                }
            }
            return competition.Matches;
        }
    }
}
