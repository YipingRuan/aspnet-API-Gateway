using Common.ErrorHandling;
using System.Collections;
using System.Text.Json;

namespace WebCommon.CodedErrorHelper
{
    /// <summary>
    /// Pass between service, preserve intenral details
    /// [CodedException] <-> [CodedError] -> [ClientCodedError]
    /// </summary>
    public class CodedError
    {
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public string Code { get; set; }
        public IDictionary Data { get; set; }
        public string Message { get; set; }
        public IDictionary InternalDetails { get; set; }

        /// <summary>
        /// Optionally keep InternalDetails, message need to be translated.
        /// </summary>
        /// <param name="keepInternalDetails"></param>
        /// <returns></returns>
        public ClientErrorResponse ToClientErrorResponse(bool keepInternalDetails = false)
        {
            var result = new ClientErrorResponse
            {
                CorrelationId = CorrelationId,
                TimeStamp = TimeStamp,
                Code = Code,
                InternalDetails = keepInternalDetails ? InternalDetails : null,
            };

            return result;
        }

        /// <summary>
        /// Recover an exception to be thrown, during HttpClient call in services
        /// </summary>
        /// <returns></returns>
        public CodedException ToException()
        {
            var ex = new CodedException(Code, Message, Data)
            {
                TimeStamp = TimeStamp,
                InternalDetails = InternalDetails,
            };

            return ex;
        }
    }

    public static class CodedExceptionExtention
    {
        /// <summary>
        /// Only used in CodedErrorMiddleware when sending error cross HTTP
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public static CodedError ToCodedError(this CodedException ex, string correlationId)
        {
            ex.InternalDetails ??= ex.ExtractInternalDetails();  // Only available after throw

            var response = new CodedError
            {
                CorrelationId = correlationId, // Get from HTTP context
                TimeStamp = ex.TimeStamp,
                Code = ex.Code,
                Data = ex.Data,
                Message = ex.Message,
                InternalDetails = ex.InternalDetails,
            };

            return response;
        }
    }
}
