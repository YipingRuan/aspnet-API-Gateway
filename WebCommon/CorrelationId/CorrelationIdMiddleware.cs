using Microsoft.AspNetCore.Http;

namespace WebCommon.CorrelationId
{
    public class CorrelationIdMiddleware
    {
        public static readonly AsyncLocal<string> Id = new AsyncLocal<string>();
        public static readonly List<Action<string>> OnCorrelationReadyActions = new();

        static readonly string IdHeader = "CorrelationId";
        static string GenerateId() => Guid.NewGuid().ToString("N").Substring(0, 5);

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string correlationId = context.Request.Headers[IdHeader].ToString();
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = GenerateId();
            }

            Id.Value = correlationId;
            context.Response.Headers.Add(IdHeader, correlationId);
            foreach (var a in OnCorrelationReadyActions)
            {
                a(correlationId);
            }

            await _next.Invoke(context);
        }
    }
}
