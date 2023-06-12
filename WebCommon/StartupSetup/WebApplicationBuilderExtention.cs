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
    public static class WebApplicationBuilderExtention
    {
        public static WebApplicationBuilder CommonSetup(this WebApplicationBuilder builder, string applicationName)
        {
            LoadConfig(builder);

            // Regular asp.net
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            SetupLogging(builder, applicationName);

            return builder;
        }

        private static void LoadConfig(WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath(AppContext.BaseDirectory);
            string[] files = { "shared-settings", "shared-settings.local", "appsettings.local" };
            foreach (var item in files)
            {
                builder.Configuration.AddJsonFile($"{item}.json", optional: true);
            }
            
            builder.Configuration.AddEnvironmentVariables();
        }

        private static void SetupLogging(WebApplicationBuilder builder, string applicationName)
        {
            builder.SetupSerilog(applicationName);
            CorrelationIdMiddleware.OnIdReadyActions.Add(id => LogContext.PushProperty("CorrelationId", id));
        }
    }
}
