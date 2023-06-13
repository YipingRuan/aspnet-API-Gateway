using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebCommon.MicroserviceUpstream.StartupSetup
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// </summary>
    public static class WebApplicationExtention
    {
        public static void CommonSetup(this WebApplication app)
        {
            app.MapControllers();
            app.MapHealthChecks("/health");

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CodedErrorMiddleware>();

            LogUtility.SetLoggerFactory(app.Services.GetService<ILoggerFactory>());
        }
    }
}
