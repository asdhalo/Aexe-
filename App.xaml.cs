using System.Windows;
using Application = System.Windows.Application;
using Aexe.Resources;

namespace Aexe
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CreateDefaultImage.CreateAndSave();
        }
    }
}