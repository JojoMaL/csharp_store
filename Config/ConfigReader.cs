using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SeleniumFramework.Config
{
    public class ConfigReader
    {
        private static JObject? _config;

        static ConfigReader()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            _config = JObject.Parse(json);
        }

        public static string? GetValue(string key)
        {
            return _config?[key]?.ToString();
        }

        public static string? BaseUrl => GetValue("baseUrl");
        public static string? Browser => GetValue("browser");
    }
}