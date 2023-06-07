using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace WebCommon.Logging
{
    /// <summary>
    /// Use in asp.net core Program.cs. Shared setup for all API projects.
    /// Like builder.CommonSetup();
    /// </summary>
    public static class SerilogSetupExtention
    {
        static string ConsoleTemplate = "[{Timestamp:HH:mm:ss}][{Level:u3}] {Message}{NewLine}{Exception}";
        //static string FileTemplate = "[{Timestamp:HH:mm:ss}][{Level:u3}][{CorrelationId}][{ServiceName}] {Message}{NewLine}{Exception}";

        public static void SetupSerilog(this WebApplicationBuilder builder, string serviceName)
        {
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)

                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)

                .WriteTo.Console(outputTemplate: ConsoleTemplate)
                .WriteTo.Debug()

                .ReadFrom.Services(services)
                .ReadFrom.Configuration(context.Configuration));
        }
    }
}