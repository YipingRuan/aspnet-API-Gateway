using Microsoft.AspNetCore.Builder;
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
        }
    }
}
