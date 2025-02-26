using Aexe.Commands;
using Aexe.Models;
using Aexe.Services;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Aexe.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Window _window;
        private readonly SettingsService _settingsService;
        private string? _playerPath;
        private string _playerArguments = "\"{0}\"";
        private string _newPath = string.Empty;
        
        public ObservableCollection<string> VideoPaths { get; } = new();
        public string? PlayerPath
        {
            get => _playerPath;
            set => SetProperty(ref _playerPath, value);
        }

        public string PlayerArguments
        {
            get => _playerArguments;
            set => SetProperty(ref _playerArguments, value);
        }

        private RelayCommand? _addVideoPathCommand;
        public ICommand AddVideoPathCommand => _addVideoPathCommand ??= new RelayCommand(AddVideoPath, CanAddVideoPath);

        public string NewPath
        {
            get => _newPath;
            set
            {
                if (SetProperty(ref _newPath, value))
                {
                    _addVideoPathCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand BrowsePlayerCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RemoveVideoPathCommand { get; }
        public ICommand BrowsePathCommand { get; }

        public SettingsViewModel(Window window)
        {
            _window = window;
            _settingsService = new SettingsService();

            // 加载设置
            var settings = _settingsService.LoadSettings();
            PlayerPath = settings.PlayerPath;
            PlayerArguments = settings.PlayerArguments ?? "\"{0}\"";
            foreach (var path in settings.VideoPaths)
            {
                VideoPaths.Add(path);
            }

            // 初始化命令
            BrowsePlayerCommand = new RelayCommand(BrowsePlayer);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => _window.DialogResult = false);
            RemoveVideoPathCommand = new RelayCommand<string>(RemoveVideoPath);
            BrowsePathCommand = new RelayCommand(BrowsePath);
        }

        private void BrowsePlayer()
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "可执行文件|*.exe|所有文件|*.*",
                Title = "选择播放器"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PlayerPath = dialog.FileName;
            }
        }

        private void BrowsePath()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "选择视频目录"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                NewPath = dialog.SelectedPath;
            }
        }

        private bool CanAddVideoPath()
        {
            return !string.IsNullOrWhiteSpace(NewPath) && !VideoPaths.Contains(NewPath);
        }

        private void AddVideoPath()
        {
            if (CanAddVideoPath())
            {
                VideoPaths.Add(NewPath);
                NewPath = string.Empty;
            }
        }

        private void RemoveVideoPath(string? path)
        {
            if (path != null && VideoPaths.Contains(path))
            {
                VideoPaths.Remove(path);
            }
        }

        private void Save()
        {
            var settings = new AppSettings
            {
                PlayerPath = PlayerPath,
                PlayerArguments = PlayerArguments,
                VideoPaths = VideoPaths.ToList()
            };

            _settingsService.SaveSettings(settings);
            _window.DialogResult = true;
        }
    }
} 