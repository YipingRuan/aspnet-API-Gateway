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
        public Dictionary<string, object> InternalDetails { get; set; }

        public CodedException(string code, string message, object data = null)
            : base(message)
        {
            Code = code;
            foreach (var p in Helper.ConverToDictionary(data))
            {
                Data.Add(p.Key, p.Value);
            }
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
            if (ex is CodedException && reuseCurrentCodedException)
            {
                return ex as CodedException;
            }

            var e = new CodedException(code, ex.Message, ex.Data);
            e.InternalDetails = Helper.ExtractInternalDetails(ex);

            foreach (var p in Helper.ConverToDictionary(data))
            {
                e.Data.Add(p.Key, p.Value);
            }

            return e;
        }
    }

    static class Helper
    {
        public static Dictionary<string, object> ConverToDictionary(object obj)
        {
            if (obj == null)
            {
                return new Dictionary<string, object>();
            }

            var json = JsonSerializer.SerializeToUtf8Bytes(obj);
            var d = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return d;
        }

        public static Dictionary<string, object> ExtractInternalDetails(Exception ex)
        {
            var details = new
            {
                Module = ex.TargetSite.Module.FullyQualifiedName,
                Summary = ex.ToString().Split("\n", StringSplitOptions.TrimEntries),
            };

            return ConverToDictionary(details);
        }
    }
}