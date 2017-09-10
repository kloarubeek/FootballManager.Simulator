using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FM.Domain.Services
{
    public interface IStatisticsService
    {
        void ShowStats(List<Competition> rankingHistory);
        void ShowHistory(List<Competition> rankingHistory);
    }

    public class StatisticsService : IStatisticsService
    {
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(ILogger<StatisticsService> logger)
        {
            _logger = logger;
        }

        public void ShowStats(List<Competition> rankingHistory)
        {
            var maxMatch = rankingHistory.SelectMany(x => x.Matches).OrderByDescending(x => x.TotalGoals).Take(1).FirstOrDefault();
            var draws = rankingHistory.SelectMany(x => x.Matches).Count(x => x.WinningTeam == null);
            var homeTeamWins = rankingHistory.SelectMany(x => x.Matches).Count(x => x.WinningTeam == x.HomeTeam);
            var awayTeamWins = rankingHistory.SelectMany(x => x.Matches).Count(x => x.WinningTeam == x.AwayTeam);

            _logger.LogInformation($"Max result: {maxMatch.HomeTeam} - {maxMatch.AwayTeam} {maxMatch.Score}");
            _logger.LogInformation($"Home team wins: {homeTeamWins}");
            _logger.LogInformation($"Away team wins: {awayTeamWins}");
            _logger.LogInformation($"Draws: {draws}");

            var mostScores = rankingHistory.SelectMany(x => x.Matches).GroupBy(x => x.Score).OrderByDescending(x => x.Count()).Take(10).Select(x => new { Score = x.Key, Occurences = x.Count() });
            _logger.LogInformation("Top 10 results:");
            foreach (var score in mostScores)
            {
                _logger.LogInformation($"{score.Score}: {score.Occurences}x");
            }
        }

        public void ShowHistory(List<Competition> rankingHistory)
        {
            var teams = rankingHistory.FirstOrDefault().Teams;

            foreach (var team in teams.OrderByDescending(x => x.Strength))
            {
                var i = rankingHistory.SelectMany(x => x.Rankings).Count(x => x.Team.TeamId == team.TeamId && x.Position == 1);
                _logger.LogInformation($"{team.Name} with strength {team.Strength} became champions {i}x");
            }
        }

    }
}
