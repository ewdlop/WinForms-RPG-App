using System;
using System.IO;
using System.Text.Json;

namespace WinFormsApp1.Configuration
{
    public static class ConfigurationManager
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WinFormsApp1",
            "config.json"
        );

        private static AppConfig _config;

        static ConfigurationManager()
        {
            LoadConfiguration();
        }

        public static string Language
        {
            get => _config?.Language ?? "";
            set
            {
                if (_config == null)
                    _config = new AppConfig();
                
                _config.Language = value;
                SaveConfiguration();
            }
        }

        private static void LoadConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    var json = File.ReadAllText(ConfigFilePath);
                    _config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
                else
                {
                    _config = new AppConfig();
                }
            }
            catch
            {
                _config = new AppConfig();
            }
        }

        private static void SaveConfiguration()
        {
            try
            {
                var directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(ConfigFilePath, json);
            }
            catch
            {
                // Ignore save errors - configuration is not critical
            }
        }

        private class AppConfig
        {
            public string Language { get; set; } = "";
        }
    }
} 