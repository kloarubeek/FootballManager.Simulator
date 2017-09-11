using System;
using Microsoft.Extensions.Logging;

namespace FM.Simulator
{
    /// <summary>
    /// Custom console logger to get rid of the default 2-line logging 
    /// </summary>
    public class CustomConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public CustomConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            Console.WriteLine($"{formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
