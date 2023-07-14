namespace WebCommon
{
    public static class HttpHeaders
    {
        public const string CorrelationId = "X-T2-CorrelationId";
        public const string PreferedLanguage = "X-T2-PreferedLanguage";
        public const string ApiKey = "X-T2-APIKEY";
        public const string User = "X-T2-User";
        public const string Tenant = "X-T2-Tenant";

        /// <summary>
        /// Not from outside request, to be clear/set at Gateway only. 
        /// </summary>
        public static string[] InternalHeaders = { User, Tenant };
    }
}
