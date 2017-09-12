using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FM.Domain.Services;
using FM.Domain.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace FM.Simulator
{
    class Program
    {
        private static ILogger<Program> _logger;
        private static List<int> _teamStrengths;

        public static void Main(string[] args)
        {
            var commandLineApplication = GetCommandLineApplication();
            commandLineApplication.Execute(args);
        }

        /// <summary>
        /// Handle command line arguments
        /// </summary>
        private static CommandLineApplication GetCommandLineApplication()
        {
            var commandLineApplication = new CommandLineApplication(false);
            var teamstrengthsArgument = commandLineApplication.Option(
                "-$|-g |--teamStrengths <1,2,3,4>",
                "a comma-separated list of strength (4 teams), i.e. 40,50,20,10. Default strength is 100.",
                CommandOptionType.SingleValue);
            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                if (teamstrengthsArgument.TryParseTeamStrength(out _teamStrengths))
                {
                    Run();
                }
                else
                {
                    Console.WriteLine("Teamstrengths is not a list of numeric values. Bye bye...");
                }
                return 0;
            });
            return commandLineApplication;
        }

        private static void Run()
        {
            //setup DI
            var serviceProvider = new ServiceCollection()
                .ConfigureServices(_teamStrengths)
                .BuildServiceProvider();

            //configure console logging
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<Program>();

            var competitionService = serviceProvider.GetService<ICompetitionService>();
            var statistics = Simulate(competitionService);

            var statisticsService = serviceProvider.GetService<IStatisticsService>();
            ShowStats(statisticsService, statistics);

            Console.ReadLine();
        }

        private static List<Competition> Simulate(ICompetitionService competitionService)
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
                    competitionService.SimulateMatches(competition);
                    competitionService.LogStats(competition);
                    rankingHistory.Add(competition);
                }
                stopwatch.Stop();

                _logger.LogInformation($"It took me {stopwatch.ElapsedMilliseconds} ms. to simulate.");
                Console.Write("Again? [y] [n] ");
                runAgain = Console.ReadKey().KeyChar == 'y';
                Console.WriteLine();
            } while (runAgain);

            Console.WriteLine($"{rankingHistory.Count} took me {stopwatch.ElapsedMilliseconds} ms.");
            return rankingHistory;
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

        private static void ShowStats(IStatisticsService statisticsService, List<Competition> statistics)
        {
            statisticsService.ShowHistory(statistics);
            statisticsService.ShowStats(statistics);
        }
    }
}
