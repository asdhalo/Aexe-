namespace Aexe.Models
{
    public class AppSettings
    {
        public string? PlayerPath { get; set; }
        public string? LastScanPath { get; set; }
        public string? PlayerArguments { get; set; } = "\"{0}\"";  // {0} 将被替换为视频路径
        public List<string> VideoPaths { get; set; } = new();
    }
} 