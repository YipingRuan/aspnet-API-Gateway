using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebCommon
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
            foreach (var p in ConverToDictionary(data))
            {
                Data.Add(p.Key, p.Value);
            }
        }

        public CodedError ToCodedError(string correlationId)
        {
            if (InternalDetails == null)
            {
                InternalDetails = ExtractInternalDetails(this);  // Only available after throw
            }

            var response = new CodedError
            {
                CorrelationId = correlationId, // Get from HTTP context
                TimeStamp = TimeStamp,
                Code = Code,
                Data = ConverToDictionary(Data),
                Message = Message,
                InternalDetails = InternalDetails,
            };

            return response;
        }

        static Dictionary<string, object> ConverToDictionary(object obj)
        {
            if (obj == null)
            {
                return new Dictionary<string, object>();
            }

            var json = JsonSerializer.SerializeToUtf8Bytes(obj);
            var d = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return d;
        }

        public static CodedException FromCodedError(CodedError error)
        {
            var ex = new CodedException(error.Code, error.Message, error.Data)
            {
                TimeStamp = error.TimeStamp,
                InternalDetails = error.InternalDetails,
            };

            return ex;
        }

        public static CodedException FromException(string code, Exception ex, object data = null, bool reuseCodedException = true)
        {
            if (ex is CodedException)
            {
                return ex as CodedException;
            }

            var e = new CodedException(code, ex.Message, ex.Data);
            e.InternalDetails = ExtractInternalDetails(ex);

            foreach (var p in ConverToDictionary(data))
            {
                e.Data.Add(p.Key, p.Value);
            }

            return e;
        }

        static Dictionary<string, object> ExtractInternalDetails(Exception ex)
        {
            var details = new
            {
                Module = ex.TargetSite.Module.FullyQualifiedName,
                Summary = ex.ToString().Split("\n", StringSplitOptions.TrimEntries),
            };

            return ConverToDictionary(details);
        }
    }

    /// <summary>
    /// Pass between service 
    /// </summary>
    public record CodedError
    {
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public string Code { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> InternalDetails { get; set; }
    }

    public record CodedErrorClientResponse
    {
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public string Code { get; set; }
        public string ClientMessage { get; set; }  // Translated!
    }
}
