using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aexe.ViewModels;

namespace Aexe.Models
{
    public class MediaItem : ViewModelBase
    {
        private bool _isSelected = true;  // 默认选中

        public string Id { get; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? CoverPath { get; set; }
        public string FolderPath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
} 