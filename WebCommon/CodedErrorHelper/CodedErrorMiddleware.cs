using Common.ErrorHandling;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using WebCommon.CorrelationId;

namespace WebCommon.CodedErrorHelper
{
    public class CodedErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public CodedErrorMiddleware(RequestDelegate next)
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
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;  // Service error always 500
                response.ContentType = MediaTypeNames.Application.Json;

                // Write CodedError
                var serviceResponse = ex
                    .Bag("General.ServiceError", new { ServiceName = context.Request.Path.Value })
                    .ToCodedError(CorrelationIdMiddleware.Id.Value);

                var utf8 = JsonSerializer.SerializeToUtf8Bytes(serviceResponse);
                await response.Body.WriteAsync(utf8);
            }
        }
    }
}