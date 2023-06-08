using Common.ErrorHandling;

namespace Gateway.ClientErrorHandling
{
    public static class CodedErrorExtention
    {
        /// <summary>
        /// Optionally keep InternalDetails, message need to be translated.
        /// </summary>
        /// <param name="keepInternalDetails"></param>
        /// <returns></returns>
        public static ClientErrorResponse ToClientErrorResponse(this CodedError ex, string languageCode, bool keepInternalDetails = false)
        {
            string translated = new TranslationService(languageCode).Translate(ex.Code, ex.Data);

            var result = new ClientErrorResponse
            {
                CorrelationId = ex.CorrelationId,
                TimeStamp = ex.TimeStamp,
                Code = ex.Code,
                Data = ex.Data,
                InternalDetails = keepInternalDetails ? ex.InternalDetails : null,
                Message = translated
            };

            return result;
        }
    }
}
