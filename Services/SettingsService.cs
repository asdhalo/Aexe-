using Aexe.Models;
using System.Text.Json;
using System.IO;

namespace Aexe.Services
{
    public class SettingsService
    {
        private const string SettingsFile = "settings.json";
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFile);
                    return JsonSerializer.Deserialize<AppSettings>(json, _options) ?? new AppSettings();
                }
                catch
                {
                    return new AppSettings();
                }
            }
            return new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, _options);
            File.WriteAllText(SettingsFile, json);
        }
    }
} 