using System.Collections;
using System.Text;
using System.Text.Json;

namespace Gateway.ClientErrorHandling
{
    public class TranslationService
    {
        /// <summary>
        /// Load from json files, use FrozenDictionary<TKey,TValue> in .net 8
        /// </summary>
        static Dictionary<string, Dictionary<string, string>> TranslationTemplates = new()
        {
            ["en-GB"] = new()
            {
                ["WeatherService.ForecastFail"] = "Failed to forecast weather file {{FileName}}",
                ["General.ServiceError"] = "Internal service error: {{ServiceName}}"
            }
        };

        /// <summary>
        /// Load from json file?
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="templates"></param>
        public static void ReplaceTranslationTemplates(string languageCode, Dictionary<string, string> templates)
        {
            TranslationTemplates[languageCode] = templates;
        }

        public string LanguageCode { get; private set; }

        public TranslationService(string languageCode = "en-GB")
        {
            LanguageCode = languageCode;
        }

        public string Translate(string code, IDictionary data)
        {
            string template = GetTranslationTemplate(code);
            if (template == null)
            {
                string json = JsonSerializer.Serialize(data);
                return $"Tranlsation is not available for [{LanguageCode}][{code}]. Raw data: {json}";
            }

            StringBuilder translated = new StringBuilder(template);
            foreach (DictionaryEntry item in data)
            {
                translated.Replace("{{" + item.Key + "}}", item.Value + "");
            }

            return translated.ToString();
        }

        string GetTranslationTemplate(string code)
        {
            return TranslationTemplates
                .GetValueOrDefault(LanguageCode, null)?
                .GetValueOrDefault(code, null);
        }
    }
}
