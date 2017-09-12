using System.Collections.Generic;
using FM.Domain.Models;

namespace FM.Domain.Services
{
    public interface ITeamService
    {
        List<Team> GetTeams();
    }

    public class TeamService : ITeamService
    {
        private readonly List<int> _teamstrengths;

        public TeamService(List<int> teamstrengths)
        {            
            _teamstrengths = teamstrengths.Count == 4 ? teamstrengths : new List<int> { 100, 100, 100, 100 };
        }

        public List<Team> GetTeams()
        {
            return new List<Team>
            {
                new Team("Ajax", _teamstrengths[0], 10) {TeamId = 1 },
                new Team("Feyenoord", _teamstrengths[1], 40) { TeamId = 2 },
                new Team("PSV", _teamstrengths[2], 30) { TeamId = 3 },
                new Team("FC Utrecht", _teamstrengths[3], 20) { TeamId = 4 }
            };
        }
    }
}
