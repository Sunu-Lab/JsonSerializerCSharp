using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Helpers
{
    public class JsonToLibary
    {
        // JSON文字列をDictionary<string, dynamic>型に変換するメソッド
        public static Dictionary<string, dynamic> ParseJson(string json)
            => ParseJsonList(json).FirstOrDefault();

        // JSON文字列をDictionary<string, dynamic>型に変換するメソッド
        public static List<Dictionary<string, dynamic>> ParseJsonList(string json)
        {
            var root = JsonSerializer.Deserialize<JsonElement>(json);

            switch (root.ValueKind)
            {
                case JsonValueKind.Object:
                    return new List<Dictionary<string, dynamic>>() { ParseJsonObj(root.GetRawText()) };
                case JsonValueKind.Array:
                    var list = JsonSerializer.Deserialize<List<JsonElement>>(root);
                    return list.SelectMany(l => ParseJsonList(l.GetRawText())).ToList();
            }
            return new List<Dictionary<string, dynamic>>();
        }

        // JSON文字列をDictionary<string, dynamic>型に変換するメソッド
        private static Dictionary<string, dynamic> ParseJsonObj(string json)
        {
            var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            return dic.Select(d => new { key = d.Key, value = ParseJsonElement(d.Value) })
                .ToDictionary(a => a.key, a => a.value);
        }
        // JsonElementの型を調べて変換するメソッド
        private static dynamic ParseJsonElement(JsonElement elem)
        {
            switch (elem.ValueKind)
            {
                case JsonValueKind.String:
                    return elem.GetString();
                case JsonValueKind.Number:
                    return elem.GetDecimal();
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.Array:
                    return elem.EnumerateArray().Select(e => ParseJsonElement(e)).ToList();
                case JsonValueKind.Object:
                    return ParseJsonObj(elem.GetRawText());
                default: throw new NotSupportedException($"{elem.GetRawText()} is not supported ");
            }
        }
    }
}
