using System.Text.Json;

namespace Aexe.Services
{
    public class VideoPathManager
    {
        private const string PathsFile = "videopaths.json";
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public List<string> LoadPaths()
        {
            if (File.Exists(PathsFile))
            {
                try
                {
                    var json = File.ReadAllText(PathsFile);
                    return JsonSerializer.Deserialize<List<string>>(json, _options) ?? new List<string>();
                }
                catch
                {
                    return new List<string>();
                }
            }
            return new List<string>();
        }

        public void SavePaths(IEnumerable<string> paths)
        {
            var json = JsonSerializer.Serialize(paths.Distinct().ToList(), _options);
            File.WriteAllText(PathsFile, json);
        }
    }
} 