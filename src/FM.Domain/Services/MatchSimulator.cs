using System;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;
using Weighted_Randomizer;

namespace FM.Domain.Services
{
    public interface IMatchSimulator
    {
        Match Simulate(Match match);
    }

    public class MatchSimulator : IMatchSimulator
    {
        private readonly ILogger<MatchSimulator> _logger;
        private readonly IMatchEventService _matchEventService;
        private readonly StaticWeightedRandomizer<Event> _eventsRandomizer;

        public MatchSimulator(ILogger<MatchSimulator> logger, IMatchEventService matchEventService)
        {
            _logger = logger;
            _matchEventService = matchEventService;
            //ok, normally we shouldn't do heavy stuff in the constructor
            _eventsRandomizer = GetEvents();
        }

        public Match Simulate(Match match)
        {
            StartMatch(match);

            _logger.LogInformation($"Start match {match.HomeTeam} - {match.AwayTeam}");

            for (var minute = 1; minute <= match.Duration; minute++)
            {
                SimulateMinute(match, minute);
            }
            _logger.LogInformation($"Match finished {match}");

            return match;
        }

        private void StartMatch(Match match)
        {
            match.Duration = GetMatchLength();
            match.HomeGoals = 0;
            match.AwayGoals = 0;
        }

        private void SimulateMinute(Match match, int minute)
        {
            var @event = _eventsRandomizer.NextWithReplacement();
            var eventTeam = GetTeam(match, @event);

            if (eventTeam == null) //no event happened, continue to next minute
            {                
                return;
            }
            var matchEvent = _matchEventService.CreateMatchEvent(minute, @event, match, eventTeam);
            match.MatchEvents.Add(matchEvent);
        }

        private Team GetTeam(Match match, Event @event)
        {
            var teamStrengthRandomizer = GetTeamStrengths(match);
            var teamAggressionRandomizer = GetTeamAggression(match);

            switch (@event.EventType)
            {
                case EventTypes.Strength:
                    return teamStrengthRandomizer.NextWithReplacement();
                case EventTypes.Aggression:
                    return teamAggressionRandomizer.NextWithReplacement();
                default:
                    return null;
            }

        }

        private static StaticWeightedRandomizer<Team> GetTeamAggression(Match match)
        {
            var teamAggressionRandomizer = new StaticWeightedRandomizer<Team>
            {
                { match.HomeTeam, match.HomeTeam.Aggression },
                { match.AwayTeam, match.AwayTeam.Aggression }
            };
            return teamAggressionRandomizer;
        }

        private static StaticWeightedRandomizer<Team> GetTeamStrengths(Match match)
        {
            var teamStrengthRandomizer = new StaticWeightedRandomizer<Team>
            {
                { match.HomeTeam, match.HomeTeam.Strength },
                { match.AwayTeam, match.AwayTeam.Strength }
            };
            return teamStrengthRandomizer;
        }

        private StaticWeightedRandomizer<Event> GetEvents()
        {
            var eventRandomizer = new StaticWeightedRandomizer<Event>();
            foreach (var @event in _matchEventService.GetEvents())
            {
                eventRandomizer.Add(@event, @event.Probability);
            }
            return eventRandomizer;
        }

        private int GetMatchLength()
        {
            const int matchLength = 90;
            return matchLength + new Random().Next(2, 5);
        }
    }
}
