using System.Collections.Generic;

namespace FM.Domain.Models
{
    public class Team : IEqualityComparer<Team>
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Number between 1 and 100
        /// </summary>
        public int Strength { get; set; }
        public int Aggression { get; set; }

        public bool Equals(Team x, Team y)
        {
            return x.TeamId == y.TeamId;
        }

        public int GetHashCode(Team team)
        {
            return team.TeamId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
