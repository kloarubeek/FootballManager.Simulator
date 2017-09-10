using System.Collections.Generic;
using System.Linq;

namespace FM.Domain.Models
{
    public class Match
    {
        public Match()
        {
            MatchEvents = new List<MatchEvent>();
        }
        public int MatchId { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public int? HomeGoals { get; set; }
        public int? AwayGoals { get; set; }
        public int? Duration { get; set; }
        public List<MatchEvent> MatchEvents { get; set; }
        public int? TotalGoals => HomeGoals + AwayGoals;
        public string Score => $"{HomeGoals} - {AwayGoals}";

        /// <summary>
        /// Is the match already played?
        /// </summary>
        public bool Played => HomeGoals.HasValue && AwayGoals.HasValue;
        public bool MatchBetween(int[] teamIds)
        {
            return teamIds.Contains(HomeTeam.TeamId) && teamIds.Contains(AwayTeam.TeamId);
        }

        public Team WinningTeam
        {
            get
            {
                if (HomeGoals.HasValue && AwayGoals.HasValue)
                {
                    if (HomeGoals > AwayGoals)
                        return HomeTeam;
                    if (AwayGoals > HomeGoals)
                        return AwayTeam;
                }
                return null;
            }
        }
    }
}
