using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Common.ErrorHandling;

namespace WebCommon.MicroserviceUpstream
{
    /// <summary>
    /// Microservice wrap any inside exception
    /// </summary>
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
                response.ContentType = MediaTypeNames.Application.Json;

                // Write CodedError
                var serviceResponse = ex
                    .Bag("General.ServiceError", new { Path = context.Request.Path.Value })
                    .ToCodedError(CorrelationIdMiddleware.Id.Value);
                response.StatusCode = serviceResponse.HttpErrorCode;

                var utf8 = JsonSerializer.SerializeToUtf8Bytes(serviceResponse);
                await response.Body.WriteAsync(utf8);
            }
        }
    }
}