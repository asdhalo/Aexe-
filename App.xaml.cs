using System.Windows;
using Application = System.Windows.Application;

namespace Aexe
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 不再需要动态创建no-image.png，因为它已经作为资源文件包含在项目中
        }
    }
}