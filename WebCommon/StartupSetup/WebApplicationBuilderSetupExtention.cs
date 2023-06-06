using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
        }
    }
}
