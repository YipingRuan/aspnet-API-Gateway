using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace WebCommon
{
    public class CodedExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CodedExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)  // If anything wrong during the Controller call
            {
                // Ensure CodedException
                CodedException coded = ex is CodedException x ? x : new("General.ServiceError", ex.Message, new { ServiceName = "????" });

                // Convert to CodedExceptionServiceRespones
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.BadRequest; // Based on Exception type?
                response.ContentType = MediaTypeNames.Application.Json;
                var correlationId = Guid.NewGuid().ToString();  // Get from context
                var serviceResponse = coded.ToCodedError(correlationId);
                
                // To response
                var utf8 = JsonSerializer.SerializeToUtf8Bytes(serviceResponse);
                await response.Body.WriteAsync(utf8);
            }
        }
    }
}