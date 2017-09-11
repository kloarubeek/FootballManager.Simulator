using FM.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FM.Domain.Services
{
    public interface IMatchService
    {
        Competition SimulateMatches(Competition competition);
    }

    public class MatchService : IMatchService
    {
        private readonly ILogger _logger;
        private readonly IStandingService _standingService;
        private readonly IMatchSimulator _matchSimulator;

        public MatchService(ILogger<MatchService> logger, IStandingService standingService, IMatchSimulator matchSimulator)
        {
            _logger = logger;
            _standingService = standingService;
            _matchSimulator = matchSimulator;
        }

        public Competition SimulateMatches(Competition competition)
        {
            _logger.LogInformation("Play matches");
            foreach (var match in competition.Matches)
            {
                _matchSimulator.Simulate(match);
                _standingService.UpdateStanding(competition, match);
            }
            _standingService.CalculateFinalPosition(competition);
            return competition;
        }
    }
}
