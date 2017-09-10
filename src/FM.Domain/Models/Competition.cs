using System.Collections.Generic;

namespace FM.Domain.Models
{
    public class Competition
    {
        public Competition()
        {
            Teams = new List<Team>();
            Matches = new List<Match>();
            Rankings = new List<CompetitionRanking>();
        }

        public string Name { get; set; }
        public List<Team> Teams { get; set; }
        public List<Match> Matches { get; set; }
        public List<CompetitionRanking> Rankings { get; set; }
    }
}
