using Aexe.ViewModels;
using System.Windows;

namespace Aexe.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(this);
        }
    }
} 