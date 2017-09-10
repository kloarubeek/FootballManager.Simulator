using FM.Domain.Models;

namespace FM.Domain.Services
{
    public interface IStandingService
    {
        void UpdateStanding(Competition competition, Match match);
        void CalculateFinalPosition(Competition competition);
    }
}