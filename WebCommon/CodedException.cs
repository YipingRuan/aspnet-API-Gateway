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
        public string OriginalStackTrace { get; set; }

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
            if (OriginalStackTrace == null)
            {
                OriginalStackTrace = StackTrace;  // Only available after throw
            }

            var response = new CodedError
            {
                CorrelationId = correlationId, // Get from HTTP context
                TimeStamp = TimeStamp,
                Code = Code,
                Data = ConverToDictionary(Data),
                Message = Message,
                OriginalStackTrace = OriginalStackTrace
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

        public static CodedException CreateFromCodedError(CodedError error)
        {
            var ex = new CodedException(error.Code, error.Message, error.Data)
            {
                TimeStamp = error.TimeStamp,
                OriginalStackTrace = error.OriginalStackTrace,
            };

            return ex;
        }

        public static CodedException CreateFromException(string code, Exception e, object data = null)
        {
            var ex = new CodedException(code, e.Message, e.Data)
            {
                OriginalStackTrace = e.StackTrace,
            };

            foreach (var p in ConverToDictionary(data))
            {
                ex.Data.Add(p.Key, p.Value);
            }

            return ex;
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
        public string OriginalStackTrace { get; set; }
    }

    public record CodedErrorClientResponse
    {
        public string CorrelationId { get; set; }
        public string TimeStamp { get; set; }
        public string Code { get; set; }
        public string ClientMessage { get; set; }  // Translated!
    }
}
