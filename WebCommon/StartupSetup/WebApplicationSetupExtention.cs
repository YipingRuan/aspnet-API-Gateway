using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebCommon.CodedErrorHelper;
using WebCommon.CorrelationId;

namespace WebCommon.StartupSetup
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// </summary>
    public static class WebApplicationExtention
    {
        public static void CommonSetup(this WebApplication app)
        {
            app.MapControllers();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CodedErrorMiddleware>();
            app.MapHealthChecks("/health");

            string source = $"{nameof(WebApplicationExtention)}.{nameof(CommonSetup)}";
            LogUtility.SetLoggerFactory(app.Services.GetService<ILoggerFactory>(), source);
        }
    }
}
