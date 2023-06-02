using Common.ErrorHandling;

namespace WebCommon.CodedErrorHelper
{
    /// <summary>
    /// Return to client by Gateway
    /// </summary>
    public class ClientErrorResponse
    {
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }  // Translated!
        public Dictionary<string, object> InternalDetails { get; set; }  // Exposed during debugging
    }

    public static class CodedErrorExtention
    {
        public static ClientErrorResponse ToClientErrorResponse(this CodedError error, bool keepInternalDetails = false)
        {
            var result = new ClientErrorResponse
            {
                CorrelationId = error.CorrelationId,
                TimeStamp = error.TimeStamp,
                Code = error.Code,
                InternalDetails = keepInternalDetails ? error.InternalDetails : null,
            };

            return result;
        }
    }
}
