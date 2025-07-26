using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using NhakhoaMyNgoc.Properties;

namespace NhakhoaMyNgoc.Utilities
{
    public static class Config
    {
        private static Dictionary<string, JsonElement> data_fetched = [];

        public static string                           admin_password      = string.Empty;
        public static string                           admin_password_salt = string.Empty;
        public static string                           volume_serial       = string.Empty;
        public static string                           root_directory      = string.Empty;

        public static List<string>                     security_questions = [];
        public static Dictionary<string, string>       security_answers   = [];
        public static Dictionary<string, string>       security_salts     = [];

        public static int                              failed_login_streak;
        public static int                              remaining_time;

        public static string                           full_path          = string.Empty;

        private static string token = string.Empty;

        public static async Task<bool> Load(bool local = false)
        {
            try
            {
                string json;
                if (local)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), "NhakhoaMyNgoc", "config.json");
                    if (!File.Exists(tempPath)) return false;
                    json = File.ReadAllText(tempPath);
                }
                else
                {
                    token = await Firebase.SignIn();
                    if (token == null) return false;
                    json = await Firebase.Load(token);
                }

                data_fetched = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

                foreach (var field in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (data_fetched.TryGetValue(field.Name, out var element))
                    {
                        var value = IOUtil.ConvertJsonElement(element, field.FieldType);
                        if (value != null)
                            field.SetValue(null, value);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task Save()
        {
            if (token == null) return;

            Dictionary<string, object> config = [];
            Type type = typeof(Config);

            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (data_fetched.TryGetValue(field.Name, out var element))
                {
                    object currentValue = field.GetValue(null)!;
                    object fetchedValue = IOUtil.ConvertJsonElement(element, field.FieldType) ?? new();

                    if (!object.Equals(currentValue, fetchedValue))
                        config[field.Name] = currentValue;
                }
            }
            await Firebase.Save(token, config, "", false);
        }
    }
}
