using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LocalSettingsGenerator
{
    public static class Extensions
    {
        public static TValue GetValueOrDefault<TValue>(this IDictionary<string, TValue> source, string key, bool caseInspecificKey = false)
        {
            if (caseInspecificKey) key = source.GetTrueKeyName(key);
            if (key == null) return default(TValue);

            if (source.ContainsKey(key)) return source[key];

            return default(TValue);
        }

        public static string GetTrueKeyName<T>(this IDictionary<string, T> source, string key)
        {
            return source.Keys.FirstOrDefault(x => string.Equals(x, key, System.StringComparison.InvariantCultureIgnoreCase));
        }

        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}