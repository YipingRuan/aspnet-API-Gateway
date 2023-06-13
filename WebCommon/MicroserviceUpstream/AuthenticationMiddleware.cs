using Microsoft.AspNetCore.Http;

namespace WebCommon.MicroserviceUpstream
{
    public class AuthenticationMiddleware
    {
        public static readonly AsyncLocal<string> UserName = new AsyncLocal<string>();
        public static readonly List<Action<string>> OnUserReadyActions = new();
        
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Process user informaiton in header?
            foreach (var a in OnUserReadyActions)
            {
                a(UserName.Value);
            }

            await _next.Invoke(context);
        }
    }
}
