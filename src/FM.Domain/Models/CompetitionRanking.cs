using System;

namespace FM.Domain.Models
{
    public class CompetitionRanking : IEquatable<CompetitionRanking>
    {
        public Competition Competition { get; set; }
        public Team Team { get; set; }
        public int Position { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public bool Equals(CompetitionRanking other)
        {
            if (other == null)
                return false;
            return Played == other.Played && Points == other.Points && GoalDifference == other.GoalDifference &&
                   GoalsFor == other.GoalsFor;
        }
    }
}
