using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Common.Logging
{
    public static class LogUtility
    {
        /// <summary>
        /// Get the prepared loggerFactory
        /// </summary>
        public static ILoggerFactory LoggerFactory { get; private set; } = new NullLoggerFactory();

        public static readonly Action<ILoggerFactory> SetLoggerFactory = f => LoggerFactory = f;
    }
}
