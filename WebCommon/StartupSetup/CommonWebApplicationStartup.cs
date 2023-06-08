using Microsoft.AspNetCore.Builder;

namespace WebCommon.StartupSetup
{
    public static class CommonWebApplicationStartup
    {
        public static void Start(string[] args)
        {
            var app = WebApplication.CreateBuilder(args).CommonSetup().Build();
            app.CommonSetup().Run();
        }
    }
}
