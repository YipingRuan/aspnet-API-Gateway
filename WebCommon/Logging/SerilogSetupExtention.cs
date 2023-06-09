﻿using Microsoft.AspNetCore.Builder;
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
        //static string FileTemplate = "[{Timestamp:HH:mm:ss}][{Level:u3}][{CorrelationId}][{Microservice}] {Message}{NewLine}{Exception}";

        public static void SetupSerilog(this WebApplicationBuilder builder, string microserviceName)
        {
            builder.Host.UseSerilog((context, services, configuration) => configuration
                // All other information not wanted
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                // Show start up information like Now listening on: "http://localhost:5055"
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)

                .Enrich.FromLogContext()
                .Enrich.WithProperty("Microservice", microserviceName)

                .WriteTo.Console(outputTemplate: ConsoleTemplate)
                .WriteTo.Debug()

                .ReadFrom.Services(services)
                .ReadFrom.Configuration(context.Configuration));
        }
    }
}