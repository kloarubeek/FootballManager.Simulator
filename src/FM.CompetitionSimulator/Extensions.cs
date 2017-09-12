using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace FM.Simulator
{
    public static class Extensions
    {
        /// <summary>
        /// Try parse command line arguments for team strengths. Defaults to 100, 100, 100, 100.
        /// </summary>
        /// <returns>True if no value or valid list of 4 numbers, else false.</returns>
        public static bool TryParseTeamStrength(this CommandOption teamstrengthsArgument, out List<int> teamStrengths)
        {
            teamStrengths = new List<int> { 100, 100, 100, 100 };
            var returnValue = true;

            if (teamstrengthsArgument.HasValue())
            {
                try
                {
                    teamStrengths = teamstrengthsArgument.Value().Split(",").Select(int.Parse).ToList();
                    returnValue = teamStrengths.Count.Equals(4);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Teamstrengths is not a list of numeric values (error: {ex.Message}.");
                    returnValue = false;
                }
            }
            return returnValue;
        }
    }
}
