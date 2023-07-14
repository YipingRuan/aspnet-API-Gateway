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

            // Clear unwanted values
            foreach (var h in HttpHeaders.InternalHeaders)
            {
                headers.Remove(h);
            }

            //// Get API key and fill user information
            //string v = headers[HttpHeaders.ApiKey].ToString();

            // Verify JWT and fill user for upstream service
            headers.Add(HttpHeaders.User, "TestUser");
            headers.Add(HttpHeaders.Tenant, "CompanyA");
        }
    }
}
