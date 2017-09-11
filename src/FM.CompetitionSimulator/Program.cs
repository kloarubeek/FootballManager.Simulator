using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FM.Domain.Services;
using FM.Domain.Models;
using System.Linq;

namespace FM.Simulator
{
    class Program
    {
        private static ILogger<Program> _logger;

        public static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddProvider(new CustomConsoleLoggerProvider()))
                .AddSingleton<ICompetitionService, CompetitionService>()
                .AddSingleton<IMatchService, MatchService>()
                .AddSingleton<IMatchSimulator, MatchSimulator>()
                .AddSingleton<IStandingService, StandingService>()
                .AddSingleton<IStatisticsService, StatisticsService>()
                .BuildServiceProvider();

            //configure console logging
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Program>();

            var competitionService = serviceProvider.GetService<ICompetitionService>();
            var matchService = serviceProvider.GetService<IMatchService>();
            var statisticsService = serviceProvider.GetService<IStatisticsService>();

            Work(competitionService, matchService, statisticsService);
        }

        private static void Work(ICompetitionService competitionService, IMatchService matchService, IStatisticsService statisticsService)
        {
            var rankingHistory = new List<Competition>();
            bool runAgain;
            var stopwatch = new Stopwatch();

            do
            {
                var numberOfSimulations = GetNumberOfSimulations();
                
                stopwatch.Start();
                for (var i = 0; i < numberOfSimulations; i++)
                {
                    _logger.LogInformation(new string('-', 80));
                    var competition = competitionService.CreateCompetition($"Eredivisie {DateTime.Now}");
                    competition = matchService.SimulateMatches(competition);

                    ShowResults(competition);
                    ShowRanking(competition);
                    rankingHistory.Add(competition);
                }
                stopwatch.Stop();

                _logger.LogInformation($"It took me {stopwatch.ElapsedMilliseconds} to simulate.");
                Console.Write("Again? [y] [n] ");
                runAgain = Console.ReadKey().KeyChar == 'y';
                Console.WriteLine();
            } while (runAgain);

            Console.WriteLine($"{rankingHistory.Count} took me {stopwatch.ElapsedMilliseconds} ms.");
            statisticsService.ShowHistory(rankingHistory);
            statisticsService.ShowStats(rankingHistory);
            Console.ReadLine();
        }

        private static int GetNumberOfSimulations()
        {
            int numberofSimulations;
            bool isNumber;
            do
            {
                Console.Write("How often do you want to simulate a competition? ");
                var userInput = Console.ReadLine();
                isNumber = int.TryParse(userInput, out numberofSimulations);

                if (!isNumber)
                {
                    Console.Write($"'{userInput}' is not a number!");
                }
            } while (!isNumber);
            return numberofSimulations;
        }

        private static void ShowRanking(Competition competition)
        {
            _logger.LogInformation(new string('_', 80));
            _logger.LogInformation("Final ranking:");
            _logger.LogInformation($"Pos {"Team",-15} P W D L P Goals");

            foreach (var ranking in competition.Rankings.OrderBy(r => r.Position).ThenByDescending(r => r.Team.Name))
            {
                _logger.LogInformation($"{ranking.Position,2}. {ranking.Team.Name,-15} {ranking.Played} {ranking.Won} {ranking.Draw} {ranking.Lost} {ranking.Points} {ranking.GoalsFor,2} - {ranking.GoalsAgainst,2}");
            }
        }

        private static void ShowResults(Competition competition)
        {
            _logger.LogInformation(new string('_', 80));
            _logger.LogInformation("Results:");

            foreach (var match in competition.Matches)
            {
                _logger.LogInformation($"{match}");
            }
        }
    }
}
