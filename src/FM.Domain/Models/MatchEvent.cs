namespace FM.Domain.Models
{
    public class MatchEvent
    {
        public int MatchEventId { get; set; }
        /// <summary>
        /// In which minute did the event happen
        /// </summary>
        public int Minute { get; set; }

        public Event Event { get; set; }
        public Team Team { get; set; }
        public Player Player { get; set; }
    }

    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
    }

    public class MatchEventGoal : MatchEvent
    {
        public MatchEventGoal(Team team, int minute, Event @event)
        {
            Team = team;
            Event = @event;
            Minute = minute;
        }
    }
}
