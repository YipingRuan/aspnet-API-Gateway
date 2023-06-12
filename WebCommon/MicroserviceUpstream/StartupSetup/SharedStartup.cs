using Microsoft.AspNetCore.Builder;

namespace WebCommon.MicroserviceUpstream.StartupSetup
{
    public static class SharedStartup
    {
        public static void Start(string[] args, string microserviceName)
        {
            var app = WebApplication.CreateBuilder(args).CommonSetup(microserviceName).Build();
            app.CommonSetup().Run();
        }
    }
}
