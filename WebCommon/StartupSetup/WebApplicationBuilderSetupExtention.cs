using Microsoft.AspNetCore.Builder;
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
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            builder.SetupSerilog("WeatherService");
            CorrelationIdMiddleware.OnIdReadyActions.Add(id => LogContext.PushProperty("CorrelationId", id));

            return builder;
        }
    }
}
