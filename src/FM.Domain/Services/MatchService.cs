using System;
using System.Collections.Generic;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;
using Weighted_Randomizer;

namespace FM.Domain.Services
{
    public interface IMatchService
    {
        Competition PlayMatches(Competition competition);
    }

    public class MatchService : IMatchService
    {
        private readonly ILogger _logger;
        private readonly IStandingService _standingService;

        private static readonly List<Event> Events = new List<Event>
        {
            new Event("Yellow card", EventTypes.Aggression, 3),
            new Event("Red card", EventTypes.Aggression, 1),
            new Event("Goal", EventTypes.Strength, 2),
            new Event("Injury", EventTypes.Strength, 1),
            new Event("Nothing", EventTypes.Other, 93)
        };

        public MatchService(ILogger<MatchService> logger, IStandingService standingService)
        {
            _logger = logger;
            _standingService = standingService;
        }

        public Competition PlayMatches(Competition competition)
        {
            Console.WriteLine("Play matches");
            foreach (var match in competition.Matches)
            {
                Simulate(match);
                _standingService.UpdateStanding(competition, match);
            }
            _standingService.CalculateFinalPosition(competition);
            return competition;
        }

        private void Simulate(Match match)
        {
            var eventRandomizer = new StaticWeightedRandomizer<Event>();
            var teamStrengthRandomizer = new StaticWeightedRandomizer<Team>();
            var teamAggressionRandomizer = new StaticWeightedRandomizer<Team>();

            foreach (var @event in Events)
            {
                eventRandomizer.Add(@event, @event.Probability);
            }
            teamStrengthRandomizer.Add(match.HomeTeam, match.HomeTeam.Strength);
            teamStrengthRandomizer.Add(match.AwayTeam, match.AwayTeam.Strength);
            teamAggressionRandomizer.Add(match.HomeTeam, match.HomeTeam.Aggression);
            teamAggressionRandomizer.Add(match.AwayTeam, match.AwayTeam.Aggression);

            match.Duration = GetMatchLength();
            match.HomeGoals = 0;
            match.AwayGoals = 0;

            Team eventTeam = null;
            Console.WriteLine($"Start match {match.HomeTeam} - {match.AwayTeam}");
            for (var minute = 1; minute < match.Duration; minute++)
            {
                var @event = eventRandomizer.NextWithReplacement();
                switch (@event.EventType)
                {
                    case EventTypes.Strength:
                        eventTeam = teamStrengthRandomizer.NextWithReplacement();
                        break;
                    case EventTypes.Aggression:
                        eventTeam = teamAggressionRandomizer.NextWithReplacement();
                        break;
                    default:
                        continue;
                }
                var matchEvent = new MatchEvent {Event = @event, Minute = minute, Team = eventTeam};
                match.MatchEvents.Add(matchEvent);
                if (@event.Description.Equals("Goal"))
                {
                    if (match.HomeTeam == eventTeam)
                    {
                        match.HomeGoals += 1;
                    }
                    else
                    {
                        match.AwayGoals += 1;
                    }
                    Console.WriteLine($"{minute} - {@event.Description} {matchEvent.Team}! {match.Score}");
                }
                else
                {
                    //red card decrease strength??
                    Console.WriteLine($"{minute} - {@event.Description} {matchEvent.Team}");
                }
            }
            Console.WriteLine($"Match finished {match.HomeTeam} - {match.AwayTeam} {match.Score}");
        }

        private int GetMatchLength()
        {
            const int matchLength = 90;
            return matchLength + new Random().Next(2, 5);
        }
    }
}
