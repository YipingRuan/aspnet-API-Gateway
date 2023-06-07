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

        /// <summary>
        /// In asp.net startup, LogUtility.SetLoggerFactory(app.Services.GetService<ILoggerFactory>(), source);
        /// </summary>
        /// <param name="newFactory"></param>
        /// <param name="remarks"></param>
        public static void SetLoggerFactory(ILoggerFactory newFactory, string source = "")
        {
            newFactory.CreateLogger(nameof(LogUtility)).LogInformation("Set LoggerFactory from {Remarks}", source);
            LoggerFactory = newFactory;
        }
    }
}
