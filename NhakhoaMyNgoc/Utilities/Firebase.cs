using NhakhoaMyNgoc.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NhakhoaMyNgoc.Utilities
{
    public static class Firebase
    {
        private static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri) { Content = content };
            return await client.SendAsync(request);
        }

        public static async Task<string> Load(string idToken)
        {
            var client = new HttpClient();
            string url = Settings.Default.FirebaseRestApi.Replace("{node}", "")
                                                         .Replace("{idToken}", idToken);
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Không đọc được Firebase");

            string config = await response.Content.ReadAsStringAsync();
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "NhakhoaMyNgoc", "config.json"), config);

            return config;
        }

        public static async Task<string> SignIn()
        {
            var client = new HttpClient();
            var requestJson = JsonSerializer.Serialize(new
            {
                email = Settings.Default.FirebaseAuthEmail,
                password = Encoding.UTF8.GetString(
                    Convert.FromBase64String(Settings.Default.FirebaseAuthPassword)),
                returnSecureToken = true
            });
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Settings.Default.FirebaseSignInAPI + Settings.Default.FirebaseAPIKey, content);

            var responseStream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream);

            if (!json.TryGetProperty("idToken", out var idTokenElement) ||
                !json.TryGetProperty("localId", out var uidElement))
                throw new Exception("Đăng nhập thất bại hoặc thiếu idToken");

            return idTokenElement.GetString()!;
        }

        public static async Task Save(string idToken, Dictionary<string, object> config, string node, bool overwrite)
        {
            string url = Settings.Default.FirebaseRestApi.Replace("{node}", node)
                                                         .Replace("{idToken}", idToken);
            string json = JsonSerializer.Serialize(config);

            var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = overwrite ? await client.PutAsync(url, content) :
                                                       await client.PatchAsync(url, content);

            response.EnsureSuccessStatusCode();
        }

        public static object ConvertJsonElement(JsonElement element, Type targetType)
        {
            try
            {
                if (targetType == typeof(string)) return element.GetString();
                if (targetType == typeof(int)) return element.GetInt32();
                if (targetType == typeof(bool)) return element.GetBoolean();
                if (targetType == typeof(double)) return element.GetDouble();
                if (targetType == typeof(List<string>))
                {
                    var list = new List<string>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(item.GetString());
                    return list;
                }
                // Add more types as needed
            }
            catch { }

            return null; // fallback
        }
    }
}
