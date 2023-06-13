using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using WebCommon.Logging;

namespace WebCommon.MicroserviceUpstream.StartupSetup
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// </summary>
    public static class WebApplicationBuilderExtention
    {
        public static void CommonSetup(this WebApplicationBuilder builder, string applicationName)
        {
            // Regular asp.net
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            builder.LoadConfig();
            builder.SetupLogging(applicationName);
        }

        static void LoadConfig(this WebApplicationBuilder builder)
        {
            builder.Configuration.SetBasePath(AppContext.BaseDirectory);
            string[] files = { "shared-settings", "shared-settings.local", "appsettings.local" };
            foreach (var item in files)
            {
                builder.Configuration.AddJsonFile($"{item}.json", optional: true);
            }

            builder.Configuration.AddEnvironmentVariables();
        }

        static void SetupLogging(this WebApplicationBuilder builder, string microserviceName)
        {
            builder.SetupSerilog(microserviceName);
            CorrelationIdMiddleware.OnIdReadyActions.Add(id => LogContext.PushProperty("CorrelationId", id));
        }
    }
}
