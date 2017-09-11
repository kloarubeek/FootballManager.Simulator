using System;
using System.Collections.Generic;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;
using Weighted_Randomizer;

namespace FM.Domain.Services
{
    public class MatchSimulator : IMatchSimulator
    {
        private static readonly List<Event> Events = new List<Event>
        {
            new Event("Yellow card", EventTypes.Aggression, 3),
            new Event("Red card", EventTypes.Aggression, 1),
            new Event("Goal", EventTypes.Strength, 2),
            new Event("Injury", EventTypes.Strength, 1),
            new Event("Nothing", EventTypes.Other, 93)
        };
        private Match _match;
        private readonly ILogger<MatchSimulator> _logger;
        private readonly StaticWeightedRandomizer<Event> _eventsRandomizer;

        public MatchSimulator(ILogger<MatchSimulator> logger)
        {
            _logger = logger;
            _eventsRandomizer = GetEvents();
        }

        public void Simulate(Match match)
        {
            _match = match;

            StartMatch();

            _logger.LogInformation($"Start match {_match.HomeTeam} - {_match.AwayTeam}");

            for (var minute = 1; minute < _match.Duration; minute++)
            {
                SimulateMinute(minute);
            }
            _logger.LogInformation($"Match finished {_match}");
        }

        private void StartMatch()
        {
            _match.Duration = GetMatchLength();
            _match.HomeGoals = 0;
            _match.AwayGoals = 0;
        }

        private void SimulateMinute(int minute)
        {
            var @event = _eventsRandomizer.NextWithReplacement();
            var eventTeam = GetTeam(@event);

            if (eventTeam == null) //no event happened, continue to next minute
            {                
                return;
            }
            var matchEvent = CreateMatchEvent(minute, @event, eventTeam);
            _match.MatchEvents.Add(matchEvent);
        }

        private MatchEvent CreateMatchEvent(int minute, Event @event, Team team)
        {
            var matchEvent = new MatchEvent { Event = @event, Minute = minute, Team = team };
            if (@event.Description.Equals("Goal"))
            {
                if (_match.HomeTeam == team)
                {
                    _match.HomeGoals += 1;
                }
                else
                {
                    _match.AwayGoals += 1;
                }
                _logger.LogInformation($"{minute} - {@event.Description} {matchEvent.Team}! {_match.Score}");
            }
            else
            {
                //red card decrease strength??
                _logger.LogInformation($"{minute} - {@event.Description} {matchEvent.Team}");
            }

            return matchEvent;
        }

        private Team GetTeam(Event @event)
        {
            var teamStrengthRandomizer = GetTeamStrengths(_match);
            var teamAggressionRandomizer = GetTeamAggression(_match);

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

        private static StaticWeightedRandomizer<Event> GetEvents()
        {
            var eventRandomizer = new StaticWeightedRandomizer<Event>();
            foreach (var @event in Events)
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
