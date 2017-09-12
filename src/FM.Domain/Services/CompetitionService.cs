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
        Competition SimulateMatches(Competition competition);
        void LogStats(Competition competition);
    }

    public class CompetitionService : ICompetitionService
    {
        private readonly ILogger _logger;
        private readonly ITeamService _teamsService;
        private readonly IMatchSimulator _matchSimulator;
        private readonly IStandingService _standingService;

        public CompetitionService(ILogger<CompetitionService> logger, ITeamService teamsService, IMatchSimulator matchSimulator, IStandingService standingService)
        {
            _logger = logger;
            _teamsService = teamsService;
            _matchSimulator = matchSimulator;
            _standingService = standingService;
        }

        public Competition CreateCompetition(string name)
        {
            _logger.LogInformation($"Start competition {name}.");

            var competition = new Competition
            {
                Name = name,
                Teams = _teamsService.GetTeams()
            };

            foreach (var team in competition.Teams)
            {
                competition.Rankings.Add(new CompetitionRanking { Team = team });
            }
            CreateMatchSchedule(competition);
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

        public Competition SimulateMatches(Competition competition)
        {
            _logger.LogInformation("Play matches");
            foreach (var match in competition.Matches)
            {
                var playedMatch = _matchSimulator.Simulate(match);
                _standingService.UpdateStanding(competition, playedMatch);
            }
            _standingService.CalculateFinalPosition(competition);
            return competition;
        }

        public void LogStats(Competition competition)
        {
            ShowResults(competition);
            ShowRanking(competition);
        }

        private void ShowRanking(Competition competition)
        {
            _logger.LogInformation(new string('_', 80));
            _logger.LogInformation("Final ranking:");
            _logger.LogInformation($"Pos {"Team",-15} P W D L P Goals");

            foreach (var ranking in competition.Rankings.OrderBy(r => r.Position).ThenByDescending(r => r.Team.Name))
            {
                _logger.LogInformation($"{ranking.Position,2}. {ranking.Team.Name,-15} {ranking.Played} {ranking.Won} {ranking.Draw} {ranking.Lost} {ranking.Points} {ranking.GoalsFor,2} - {ranking.GoalsAgainst,2}");
            }
        }

        private void ShowResults(Competition competition)
        {
            _logger.LogInformation(new string('_', 80));
            _logger.LogInformation("Results:");

            foreach (var match in competition.Matches)
            {
                _logger.LogInformation($"{match}");
            }
        }
    }
}
