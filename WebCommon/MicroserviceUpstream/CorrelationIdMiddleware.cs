using Microsoft.AspNetCore.Http;

namespace WebCommon.MicroserviceUpstream
{
    public class CorrelationIdMiddleware
    {
        public static readonly AsyncLocal<string> Id = new AsyncLocal<string>();
        public static readonly List<Action<string>> OnIdReadyActions = new();

        static string GenerateId() => Guid.NewGuid().ToString("N").Substring(0, 5);

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string correlationId = context.Request.Headers[HttpHeaders.CorrelationId].ToString();
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = GenerateId();
            }

            Id.Value = correlationId;
            context.Response.Headers.Add(HttpHeaders.CorrelationId, correlationId);

            foreach (var a in OnIdReadyActions)
            {
                a(correlationId);
            }

            await _next.Invoke(context);
        }
    }
}
