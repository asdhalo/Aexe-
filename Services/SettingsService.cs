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
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        // 缓存设置，避免频繁读取文件
        private AppSettings? _cachedSettings;

        public AppSettings LoadSettings()
        {
            // 如果已有缓存，直接返回缓存的设置
            if (_cachedSettings != null)
                return _cachedSettings;
                
            if (File.Exists(SettingsFile))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFile);
                    _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json, _options) ?? new AppSettings();
                    return _cachedSettings;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"加载设置时出错: {ex.Message}");
                    _cachedSettings = new AppSettings();
                    return _cachedSettings;
                }
            }
            
            _cachedSettings = new AppSettings();
            return _cachedSettings;
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, _options);
                
                // 使用临时文件写入，然后重命名，避免写入过程中的文件损坏
                var tempFile = SettingsFile + ".tmp";
                File.WriteAllText(tempFile, json);
                
                if (File.Exists(SettingsFile))
                    File.Delete(SettingsFile);
                    
                File.Move(tempFile, SettingsFile);
                
                // 更新缓存
                _cachedSettings = settings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存设置时出错: {ex.Message}");
                throw;
            }
        }
    }
}