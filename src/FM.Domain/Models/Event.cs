namespace FM.Domain.Models
{
    public enum EventTypes
    {
        Strength = 1,
        Aggression = 2,
        Other
    }

    public class Event
    {
        public Event(string description, EventTypes eventType, short probability)
        {
            Description = description;
            Probability = probability;
            EventType = eventType;
        }

        public int EventId { get; set; }
        public string Description { get; set; }
        public EventTypes EventType { get; set; }
        /// <summary>
        /// On a range from 1-100, what are the changes that this event occurs during a match?
        /// </summary>
        public short Probability { get; set; }
    }
}
