using System.Collections.Generic;
using FM.Domain.Models;
using Microsoft.Extensions.Logging;

namespace FM.Domain.Services
{
    public interface IMatchEventService
    {
        List<Event> GetEvents();
        MatchEvent CreateMatchEvent(int minute, Event @event, Match match, Team team);
    }

    public class MatchEventService : IMatchEventService
    {
        private readonly ILogger<MatchEventService> _logger;

        public MatchEventService(ILogger<MatchEventService> logger)
        {
            _logger = logger;
        }

        public List<Event> GetEvents()
        {
            return new List<Event>
            {
                new Event("Yellow card", EventTypes.Aggression, 3),
                new Event("Red card", EventTypes.Aggression, 1),
                new Event("Goal", EventTypes.Strength, 2),
                new Event("Injury", EventTypes.Strength, 1),
                new Event("Nothing", EventTypes.Other, 93)
            };
        }

        public MatchEvent CreateMatchEvent(int minute, Event @event, Match match, Team team)
        {
            var matchEvent = new MatchEvent { Event = @event, Minute = minute, Team = team };

            //hmm, checking on description is not the best approach
            if (@event.Description.Equals("Goal"))
            {
                if (match.HomeTeam == team)
                {
                    match.HomeGoals += 1;
                }
                else
                {
                    match.AwayGoals += 1;
                }
                _logger.LogInformation($"{minute} - {@event.Description} {matchEvent.Team}! {match.Score}");
            }
            else
            {
                //red card decrease strength??
                _logger.LogInformation($"{minute} - {@event.Description} {matchEvent.Team}");
            }

            return matchEvent;
        }

    }
}
