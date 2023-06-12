using Microsoft.AspNetCore.Http;

namespace WebCommon.MicroserviceGateway
{
    // Convert JWT/API-key to request context in header
    public class GatewayAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public GatewayAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            ProcessAuthentication(context);
            
            await _next.Invoke(context);
        }

        public void ProcessAuthentication(HttpContext context)
        {
            var headers = context.Request.Headers;

            // Get API key and fill user information
            string v = headers["test"].ToString();
            headers.Add("test_added", v + " added");

            if (headers[HttpHeaders.PreferedLanguage].Count == 0)
            {
                headers[HttpHeaders.PreferedLanguage] = "en-GB";
            }
            else
            {
                // Use user's language setting
            }
        }
    }
}
