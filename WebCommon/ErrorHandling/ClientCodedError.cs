using System.Collections;

namespace WebCommon.ClientErrorHandling
{
    /// <summary>
    /// Return to client by Gateway
    /// </summary>
    public class ClientErrorResponse
    {
        public string Message { get; set; }  // Translated!
        public string Code { get; set; }
        public IDictionary Data { get; set; }
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public IDictionary InternalDetails { get; set; }  // Exposed during debugging
    }
}
