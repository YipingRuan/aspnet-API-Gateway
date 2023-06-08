using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using WebCommon.CorrelationId;
using WebCommon.Logging;

namespace WebCommon.StartupSetup
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// </summary>
    public static class WebApplicationBuilderSetupExtention
    {
        public static WebApplicationBuilder CommonSetup(this WebApplicationBuilder builder)
        {
            // Load config
            string[] jsons = { "shared-settings", "shared-settings.local", "appsettings.local" };
            foreach (var item in jsons)
            {
                builder.Configuration.AddJsonFile($"{item}.json", optional: true);
            }
            builder.Configuration.AddEnvironmentVariables();

            // Regular asp.net
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            // Setup Serilog
            builder.SetupSerilog("WeatherService");
            CorrelationIdMiddleware.OnIdReadyActions.Add(id => LogContext.PushProperty("CorrelationId", id));

            return builder;
        }
    }
}
