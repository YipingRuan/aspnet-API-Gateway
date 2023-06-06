using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebCommon.CodedErrorHelper;
using WebCommon.CorrelationId;

namespace WebCommon.Extentions
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// Like builder.CommonSetup();
    /// </summary>
    public static class CommonSetupExtention
    {
        public static void CommonSetup(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
        }

        public static void CommonSetup(this WebApplication app)
        {
            app.MapControllers();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<CodedErrorMiddleware>();
            app.MapHealthChecks("/health");
        }
    }
}
