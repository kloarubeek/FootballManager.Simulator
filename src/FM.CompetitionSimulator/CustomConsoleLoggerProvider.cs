using Microsoft.Extensions.Logging;

namespace FM.Simulator
{
    public class CustomConsoleLoggerProvider : ILoggerProvider
    {
        public void Dispose() { }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomConsoleLogger(categoryName);
        }
    }
}
