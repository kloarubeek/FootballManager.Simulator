using System.Collections.Generic;
using FM.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FM.Simulator
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, List<int> teamStrengths)
        {
            services.AddLogging(builder => builder.AddProvider(new CustomConsoleLoggerProvider()));
            
            services.AddSingleton<ICompetitionService, CompetitionService>()
                .AddSingleton<IMatchSimulator, MatchSimulator>()
                .AddSingleton<IMatchEventService, MatchEventService>()
                .AddSingleton<IStandingService, StandingService>()
                .AddSingleton<IStatisticsService, StatisticsService>()
                .AddSingleton<ITeamService>(new TeamService(teamStrengths));

            return services;
        }
    }
}
