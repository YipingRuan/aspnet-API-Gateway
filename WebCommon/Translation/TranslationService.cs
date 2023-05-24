using System.Text;
using System.Text.Json;

namespace WebCommon.Translation
{
    public class TranslationService
    {
        /// <summary>
        /// Load from json files, use FrozenDictionary<TKey,TValue> in .net 8
        /// </summary>
        static Dictionary<string, Dictionary<string, string>> Translations = new()
        {
            ["en-GB"] = new()
            {
                ["WeatherService.ForecastFail"] = "Failed to forecast weather file {{FileName}}",
                ["General.ServiceError"] = "Internal service error: {{ServiceName}}"
            }
        };

        public string LanguageCode { get; private set; }

        public TranslationService(string languageCode = "en-GB")
        {
            LanguageCode = languageCode;
        }

        public string Translate(string code, Dictionary<string, object> data)
        {
            string template = GetTranslationTemplate(code);
            if (template == null)
            {
                return $"Tranlsation is not available for [{LanguageCode}][{code}]. Raw data: {JsonSerializer.Serialize(data)}";
            }

            StringBuilder translated = new StringBuilder(template);
            foreach (var item in data)
            {
                translated.Replace("{{" + item.Key + "}}", item.Value + "");
            }

            return translated.ToString();
        }

        string GetTranslationTemplate(string code)
        {
            return Translations
                .GetValueOrDefault(LanguageCode, null)?
                .GetValueOrDefault(code, null);
        }
    }
}
