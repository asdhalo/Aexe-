using System.Text.Json;

namespace Aexe.Services
{
    public class VideoPathManager
    {
        private const string PathsFile = "videopaths.json";
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        // 缓存路径列表，避免频繁读取文件
        private List<string>? _cachedPaths;

        public List<string> LoadPaths()
        {
            // 如果已有缓存，直接返回缓存的路径列表
            if (_cachedPaths != null)
                return new List<string>(_cachedPaths);
                
            if (File.Exists(PathsFile))
            {
                try
                {
                    var json = File.ReadAllText(PathsFile);
                    _cachedPaths = JsonSerializer.Deserialize<List<string>>(json, _options) ?? new List<string>();
                    return new List<string>(_cachedPaths);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"加载视频路径时出错: {ex.Message}");
                    _cachedPaths = new List<string>();
                    return new List<string>();
                }
            }
            
            _cachedPaths = new List<string>();
            return new List<string>();
        }

        public void SavePaths(IEnumerable<string> paths)
        {
            try
            {
                // 去重并创建新的列表
                var distinctPaths = paths.Distinct().ToList();
                var json = JsonSerializer.Serialize(distinctPaths, _options);
                
                // 使用临时文件写入，然后重命名，避免写入过程中的文件损坏
                var tempFile = PathsFile + ".tmp";
                File.WriteAllText(tempFile, json);
                
                if (File.Exists(PathsFile))
                    File.Delete(PathsFile);
                    
                File.Move(tempFile, PathsFile);
                
                // 更新缓存
                _cachedPaths = distinctPaths;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存视频路径时出错: {ex.Message}");
                throw;
            }
        }
    }
}