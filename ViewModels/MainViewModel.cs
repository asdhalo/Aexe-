using Aexe.Models;
using Aexe.Services;
using Aexe.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Aexe.Commands;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using System.Diagnostics;
using System.IO;
using Aexe.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WpfApplication = System.Windows.Application;

namespace Aexe.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MediaScanner _mediaScanner;
        private readonly SettingsService _settingsService;
        private AppSettings _settings;
        private bool _isScanning;
        private MediaItem? _selectedMedia;
        private bool _isAscending = true;
        private string _searchText = string.Empty;
        private ObservableCollection<MediaItem> _allItems = new();

        public ObservableCollection<MediaItem> MediaItems { get; } = new();
        
        public AppSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public bool IsAscending
        {
            get => _isAscending;
            set => SetProperty(ref _isAscending, value);
        }

        public MediaItem? SelectedMedia
        {
            get => _selectedMedia;
            set => SetProperty(ref _selectedMedia, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterItems();
                }
            }
        }

        // 命令属性
        public ICommand RefreshCommand { get; }
        public ICommand PlayVideoCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand SortByDateCommand { get; }

        public MainViewModel()
        {
            _mediaScanner = new MediaScanner();
            _settingsService = new SettingsService();
            _settings = _settingsService.LoadSettings();

            // 初始化命令
            RefreshCommand = new RelayCommand(async () => await RefreshAllAsync());
            PlayVideoCommand = new RelayCommand(PlayVideo, () => SelectedMedia != null);
            SettingsCommand = new RelayCommand(ShowSettings);
            SortByDateCommand = new RelayCommand(SortByDate);

            // 初始加载
            _ = RefreshAllAsync();
        }

        private void ShowSettings()
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = WpfApplication.Current.MainWindow
            };

            if (settingsWindow.ShowDialog() == true)
            {
                Settings = _settingsService.LoadSettings();
                _ = RefreshAllAsync();
            }
        }

        private void PlayVideo()
        {
            if (SelectedMedia?.FilePath == null || !File.Exists(SelectedMedia.FilePath)) 
            {
                MessageBox.Show("视频文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true
                };

                if (string.IsNullOrEmpty(Settings.PlayerPath))
                {
                    startInfo.FileName = SelectedMedia.FilePath;
                }
                else
                {
                    startInfo.FileName = Settings.PlayerPath;
                    startInfo.Arguments = string.Format(
                        Settings.PlayerArguments ?? "\"{0}\"", 
                        SelectedMedia.FilePath);
                }

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法播放视频: {ex.Message}", "错误", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByDate()
        {
            // 使用List<T>的排序方法而不是LINQ，减少内存分配
            var items = MediaItems.ToList();
            
            if (_isAscending)
                items.Sort((x, y) => x.CreatedDate.CompareTo(y.CreatedDate));
            else
                items.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
            
            // 使用批量更新减少UI更新次数
            using (CollectionViewSource.GetDefaultView(MediaItems).DeferRefresh())
            {
                MediaItems.Clear();
                foreach (var item in items)
                {
                    MediaItems.Add(item);
                }
            }
            
            IsAscending = !IsAscending;
        }

        private void FilterItems()
        {
            // 使用批量更新减少UI更新次数
            using (CollectionViewSource.GetDefaultView(MediaItems).DeferRefresh())
            {
                MediaItems.Clear();
                
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    foreach (var item in _allItems)
                    {
                        MediaItems.Add(item);
                    }
                }
                else
                {
                    // 预先缓存搜索文本，避免重复转换
                    var searchTextUpper = SearchText.ToUpperInvariant();
                    
                    foreach (var item in _allItems)
                    {
                        if (item.Title.IndexOf(searchTextUpper, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            item.FolderPath.IndexOf(searchTextUpper, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            MediaItems.Add(item);
                        }
                    }
                }
            }
        }

        private async Task RefreshAllAsync()
        {
            try
            {
                IsScanning = true;
                _allItems.Clear();
                MediaItems.Clear();

                var tasks = Settings.VideoPaths
                    .Where(Directory.Exists)
                    .Select(path => _mediaScanner.ScanDirectoryAsync(path));

                var results = await Task.WhenAll(tasks);
                
                foreach (var items in results)
                {
                    foreach (var item in items)
                    {
                        _allItems.Add(item);
                    }
                }

                // 应用搜索过滤
                FilterItems();
                // 应用排序
                SortByDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsScanning = false;
            }
        }
    }
}