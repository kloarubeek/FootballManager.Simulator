using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FM.Domain.Models;
using FM.Domain.Services;

namespace FM.Simulator
{
    class Program
    {
        public static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ICompetitionService, CompetitionService>()
                .AddSingleton<IMatchService, MatchService>()
                .AddSingleton<IStandingService, StandingService>()
                .AddSingleton<IStatisticsService, StatisticsService>()
                .BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Starting application");

            var competitionService = serviceProvider.GetService<ICompetitionService>();
            var matchService = serviceProvider.GetService<IMatchService>();
            var statisticsService = serviceProvider.GetService<IStatisticsService>();
            //var standingsService = serviceProvider.GetService<IStandingService>();
            //do the actual work here
            Work(competitionService, matchService, statisticsService);

            logger.LogDebug("All done!");
        }

        private static void Work(ICompetitionService competitionService, IMatchService matchService, IStatisticsService statisticsService)
        {
            var rankingHistory = new List<Competition>();
            bool yes;
            var stopwatch = new Stopwatch();

            do
            {
                Console.WriteLine("Simulate how often?");
                var count = Console.ReadLine();

                stopwatch.Start();
                for (int i = 0; i < int.Parse(count); i++)
                {
                    var competition = competitionService.CreateCompetition($"Eredivisie {DateTime.Now}");
                    var schedule = competitionService.CreateMatchSchedule(competition);

                    competition = matchService.PlayMatches(competition);
                    ShowResults(competition);
                    ShowRanking(competition);
                    rankingHistory.Add(competition);
                }
                stopwatch.Stop();
                Console.WriteLine($"took me {stopwatch.ElapsedMilliseconds}...");
                Console.Write("Again? [y] [n] ");
                yes = Console.ReadKey().KeyChar == 'y';
                Console.WriteLine();
            } while (yes);

            Console.WriteLine($"{rankingHistory.Count} took me {stopwatch.ElapsedMilliseconds} ms.");
            statisticsService.ShowHistory(rankingHistory);
            statisticsService.ShowStats(rankingHistory);
            Console.ReadLine();
        }


        private static void ShowRanking(Competition competition)
        {
            Console.WriteLine("Final ranking:");
            foreach (var ranking in competition.Rankings.OrderBy(r => r.Position).ThenByDescending(r => r.Team.Name))
            {
                var teamName = string.Format("{0,-15}", ranking.Team.Name);
                Console.WriteLine($"{ranking.Position}. {teamName} {ranking.Played} {ranking.Won} {ranking.Draw} {ranking.Lost} {ranking.Points} {ranking.GoalsFor}-{ranking.GoalsAgainst}");
            }
        }

        private static void ShowResults(Competition competition)
        {
            Console.WriteLine("Results:");

            foreach (var match in competition.Matches)
            {
                var matchje = string.Format("{0,-25}", $"{match.HomeTeam.Name} - {match.AwayTeam.Name}");
                Console.WriteLine($"{matchje} {match.HomeGoals}-{match.AwayGoals}");
            }
        }
    }
}
