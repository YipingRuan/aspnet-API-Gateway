using Microsoft.AspNetCore.Builder;

namespace WebCommon.StartupSetup
{
    public static class SharedStartup
    {
        public static void Start(string[] args, string applicationName)
        {
            var app = WebApplication.CreateBuilder(args).CommonSetup(applicationName).Build();
            app.CommonSetup().Run();
        }
    }
}
