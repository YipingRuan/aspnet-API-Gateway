using System.Collections;

namespace Common.ErrorHandling
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
