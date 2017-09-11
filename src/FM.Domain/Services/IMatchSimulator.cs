using FM.Domain.Models;

namespace FM.Domain.Services
{
    public interface IMatchSimulator
    {
        void Simulate(Match match);
    }
}