using Common.ErrorHandling;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebCommon.CodedErrorHelper;
using WebCommon.Translation;

namespace Gateway.Middlewares
{
    // https://auth0.com/blog/building-a-reverse-proxy-in-dot-net-core/
    // 1. Forward request to microservices
    // 2. Translate CodedError to CodedErrorClientResponse
    public class MicroserviceGatewayMiddleware
    {
        private static readonly HttpClient _httpClient = new();
        private readonly RequestDelegate _nextMiddleware;
        private readonly IConfiguration _config;

        public MicroserviceGatewayMiddleware(RequestDelegate nextMiddleware, IConfiguration config)
        {
            _nextMiddleware = nextMiddleware;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await ForwardRequest(context);
            }
            catch (Exception ex)  // Error during _httpClient call
            {
                var codedError = ex.Bag("ForwardRequest.Error").ToCodedError(null);
                var clientResponse = ProcessCodedError(codedError,"en-GB");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsJsonAsync(clientResponse, DefaultJsonSerializerOptions);
            }
        }

        public async Task ForwardRequest(HttpContext context)
        {
            var targetUri = BuildTargetUri(context.Request);

            if (targetUri == null || targetUri.Segments.Length == 1)
            {
                await _nextMiddleware(context);
                return;
            }

            var targetRequestMessage = CreateTargetMessage(context, targetUri);

            // API Call
            using var responseMessage = await _httpClient.SendAsync(targetRequestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                context.RequestAborted);

            context.Response.StatusCode = (int)responseMessage.StatusCode;
            CopyFromTargetResponseHeaders(context, responseMessage);

            await ProcessResponseContent(context, responseMessage);
        }

        private async Task ProcessResponseContent(HttpContext context, HttpResponseMessage responseMessage)
        {
            // Is a CodedError response? (Must be 500)
            if (responseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                var content = await responseMessage.Content.ReadAsByteArrayAsync();
                if (content.Length > 0)
                {
                    var error = JsonSerializer.Deserialize<CodedError>(content);
                    if (!string.IsNullOrEmpty(error.Code))
                    {
                        ClientErrorResponse clientResponse = ProcessCodedError(error, "en-GB");
                        await context.Response.WriteAsJsonAsync(clientResponse, DefaultJsonSerializerOptions);
                        return;
                    }
                }
            }

            // 200, 500 but not CodedError, other
            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }

        private ClientErrorResponse ProcessCodedError(CodedError error, string languageCode)
        {
            bool keepInternalDetails = _config.GetValue("ErrorHandling:ClientErrorResponseCarriesInternalDetails", false);
            var clientResponse = error.ToClientErrorResponse(keepInternalDetails);
            clientResponse.Message = new TranslationService(languageCode).Translate(error.Code, error.Data);

            return clientResponse;
        }

        private bool IsContentOfType(HttpResponseMessage responseMessage, string type)
        {
            var result = false;

            if (responseMessage.Content?.Headers?.ContentType != null)
            {
                result = responseMessage.Content.Headers.ContentType.MediaType == type;
            }

            return result;
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            context.Response.Headers.Remove("transfer-encoding");
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private Uri BuildTargetUri(HttpRequest request)
        {
            var match = RequestPathPattern.Match(request.Path);
            if (!match.Success)
            {
                return null;
            }

            var targetService = ServiceMapping[match.Groups["ServiceName"].Value];

            return new Uri($"{targetService}/{match.Groups["RemainingPath"].Value}{request.QueryString.Value}");
        }

        static JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNamingPolicy = null };

        static Regex RequestPathPattern = new(@"/(?<ServiceName>.+?)/(?<RemainingPath>.+)", RegexOptions.Compiled);

        static Dictionary<string, string> ServiceMapping = new()
        {
            ["WeatherService"] = "http://localhost:5055",
        };
    }
}
