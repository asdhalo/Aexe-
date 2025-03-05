using Aexe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aexe.Services
{
    public class MediaScanner
    {
        private static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
        private static readonly string[] CoverNames = { "fanart", "poster", "cover", "folder" };  // 按优先级排序

        public IEnumerable<MediaItem> ScanDirectory(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"目录不存在: {path}");

            var videoFiles = Directory.GetFiles(path, "*.mp4", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(path, "*.avi", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(path, "*.mkv", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(path, "*.mov", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(path, "*.wmv", SearchOption.AllDirectories));

            foreach (var videoFile in videoFiles)
            {
                var directory = Path.GetDirectoryName(videoFile) 
                    ?? throw new InvalidOperationException("无法获取目录路径");
                var fileName = Path.GetFileNameWithoutExtension(videoFile);
                var fileInfo = new FileInfo(videoFile);
                
                // 查找封面图片
                var coverFile = FindCoverImage(directory, fileName);

                var mediaItem = new MediaItem
                {
                    Title = fileName,
                    FilePath = videoFile,
                    CoverPath = coverFile,
                    FolderPath = directory,
                    FileSize = fileInfo.Length,
                    CreatedDate = fileInfo.CreationTime,
                    ModifiedDate = fileInfo.LastWriteTime
                };

                yield return mediaItem;
            }
        }

        private string? FindCoverImage(string directory, string videoFileName)
        {
            // 1. 首先查找与视频同名的图片
            var sameNameImage = _imageExtensions
                .Select(ext => Path.Combine(directory, videoFileName + ext))
                .FirstOrDefault(File.Exists);
            
            if (sameNameImage != null) return sameNameImage;

            // 2. 按优先级查找特定命名的图片
            foreach (var coverName in CoverNames)
            {
                var coverFile = Directory.GetFiles(directory)
                    .FirstOrDefault(f => 
                        ImageExtensions.Contains(Path.GetExtension(f).ToLower()) &&
                        Path.GetFileNameWithoutExtension(f).Equals(coverName, StringComparison.OrdinalIgnoreCase));
                
                if (coverFile != null) return coverFile;
            }

            // 3. 查找父目录中的封面图片
            var parentDirectory = Directory.GetParent(directory)?.FullName;
            if (parentDirectory != null)
            {
                foreach (var coverName in CoverNames)
                {
                    var parentCoverFile = Directory.GetFiles(parentDirectory)
                        .FirstOrDefault(f => 
                            ImageExtensions.Contains(Path.GetExtension(f).ToLower()) &&
                            Path.GetFileNameWithoutExtension(f).Equals(coverName, StringComparison.OrdinalIgnoreCase));
                    
                    if (parentCoverFile != null) return parentCoverFile;
                }
            }

            return null;
        }

        public async Task<IEnumerable<MediaItem>> ScanDirectoryAsync(string path)
        {
            return await Task.Run(() => ScanDirectory(path));
        }

        private readonly HashSet<string> _videoExtensions;
        private readonly HashSet<string> _imageExtensions;

        public MediaScanner()
        {
            _videoExtensions = new HashSet<string>(VideoExtensions, StringComparer.OrdinalIgnoreCase);
            _imageExtensions = new HashSet<string>(ImageExtensions, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsVideoFile(string filePath)
        {
            return _videoExtensions.Contains(Path.GetExtension(filePath));
        }

        private bool IsImageFile(string filePath)
        {
            return _imageExtensions.Contains(Path.GetExtension(filePath));
        }
    }
}
