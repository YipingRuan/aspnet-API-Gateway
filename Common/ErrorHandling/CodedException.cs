using System.Collections;
using System.Text.Json;

namespace Common.ErrorHandling
{
    /// <summary>
    /// An exception that can be translated
    /// </summary>
    public class CodedException : Exception
    {
        public string TimeStamp { get; set; } = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        public string Code { get; set; }
        public IDictionary InternalDetails { get; set; }

        public CodedException(string code, string message, object data = null, IDictionary internalDetails = null)
            : base(message)
        {
            Code = code;
            InternalDetails = internalDetails;
            
            // Add extra data
            if (data != null)
            {
                var json = JsonSerializer.SerializeToUtf8Bytes(data);
                var d = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                foreach (var p in d)
                {
                    Data[p.Key] = p.Value;
                }
            }
        }

        /// <summary>
        /// Only used in CodedErrorMiddleware when sending error cross HTTP
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public CodedError ToCodedError(string correlationId)
        {
            var internalDetails = InternalDetails ?? this.ExtractInternalDetails();  // Only available after throw

            var response = new CodedError
            {
                CorrelationId = correlationId, // Get from HTTP context
                TimeStamp = TimeStamp,
                Code = Code,
                Data = Data,
                Message = Message,
                InternalDetails = internalDetails,
            };

            return response;
        }
    }

    public static class ExceptionExtention
    {
        /// <summary>
        /// Pack Exception and extra to CodedException
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="reuseCurrentCodedException"></param>
        /// <returns></returns>
        public static CodedException Bag(this Exception ex, string code, object data = null, bool reuseCurrentCodedException = true)
        {
            if (reuseCurrentCodedException && ex is CodedException)
            {
                return ex as CodedException;
            }

            var e = new CodedException(code, ex.Message, data, ex.ExtractInternalDetails());

            return e;
        }

        public static IDictionary ExtractInternalDetails(this Exception ex)
        {
            var details = new Dictionary<string, object>
            {
                ["Module"] = ex.TargetSite.Module.FullyQualifiedName,
                ["Summary"] = ex.ToString().Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            };

            return details;
        }
    }
}