using System;
using System.Linq;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FM.Domain.Services
{
    public interface IStandingService
    {
        void UpdateStanding(Competition competition, Match match);
        void CalculateFinalPosition(Competition competition);
    }

    public class StandingService : IStandingService
    {
        private readonly ILogger _logger;

        public StandingService(ILogger<StandingService> logger)
        {
            _logger = logger;
        }

        public void UpdateStanding(Competition competition, Match match)
        {
            if (match.Played)
            {
                UpdateStandingForTeam(competition, match, match.HomeTeam);
                UpdateStandingForTeam(competition, match, match.AwayTeam);
            }
        }

        public void CalculateFinalPosition(Competition competition)
        {
            var rank = 1;
            CompetitionRanking prevTeamRanking = null;

            foreach (var teamRanking in competition.Rankings
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor))
            {
                teamRanking.Position = rank;

                if (teamRanking.Equals(prevTeamRanking))
                {
                    //find onderling result: only works if 2 teams end the same. Doesn't work for 3 teams with same points
                    var match = competition.Matches.Find(x => x.MatchBetween(new[] { teamRanking.Team.TeamId, prevTeamRanking.Team.TeamId }));
                    if (match.WinningTeam == teamRanking.Team)
                    {
                        //current team wins, so will swap with the other team
                        teamRanking.Position = prevTeamRanking.Position;
                        prevTeamRanking.Position = rank;
                    }
                    else if (match.WinningTeam == null) //a draw!
                    {
                        teamRanking.Position = prevTeamRanking.Position; //same final position
                    }
                }
                rank++;
                prevTeamRanking = teamRanking;
            }
        }

        private void UpdateStandingForTeam(Competition competition, Match match, Team team)
        {
            if (match.HomeGoals == null) throw new ArgumentException(nameof(match.HomeGoals));
            if (match.AwayGoals == null) throw new ArgumentException(nameof(match.AwayGoals));

            var ranking = competition.Rankings.Find(x => x.Team == team);
            ranking.Played += 1;
            if (match.WinningTeam == ranking.Team)
            {
                ranking.Won += 1;
                ranking.Points += 3;
            }
            else if (match.WinningTeam == null)
            {
                ranking.Draw += 1;
                ranking.Points += 1;
            }
            else
            {
                ranking.Lost += 1;
            }
            ranking.GoalsFor += match.HomeTeam == team ? match.HomeGoals.Value : match.AwayGoals.Value;
            ranking.GoalsAgainst += match.HomeTeam == team ? match.AwayGoals.Value : match.HomeGoals.Value;
        }
    }
}
