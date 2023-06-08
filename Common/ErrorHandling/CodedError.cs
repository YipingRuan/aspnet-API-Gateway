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
            var ex = new CodedException(Code, Message, Data, InternalDetails)
            {
                TimeStamp = TimeStamp,
            };

            return ex;
        }
    }
}
