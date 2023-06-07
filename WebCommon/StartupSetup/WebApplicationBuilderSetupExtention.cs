using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using WebCommon.CorrelationId;

namespace WebCommon.StartupSetup
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// </summary>
    public static class WebApplicationBuilderSetupExtention
    {
        public static void CommonSetup(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            CorrelationIdMiddleware.OnIdReadyActions.Add(id => LogContext.PushProperty("CorrelationId", id));
        }
    }
}
