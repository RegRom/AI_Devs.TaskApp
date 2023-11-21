using Microsoft.Extensions.DependencyInjection;

namespace AI_Devs.TaskApp
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = serviceProvider.GetService<MainPage>();
        }
    }
}