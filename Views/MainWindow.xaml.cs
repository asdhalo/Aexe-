using Aexe.ViewModels;
using System.Windows;

namespace Aexe.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
} 