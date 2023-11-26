using AI_Devs.TaskApp.Services.Interfaces;
using AI_Devs.TaskApp.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Net;
using System.Reflection;

namespace AI_Devs.TaskApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("AI_Devs.TaskApp.appsettings.json");
            var builder = MauiApp.CreateBuilder();

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.UseMauiApp<App>()
                .RegisterAppServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Configuration.AddConfiguration(config);
            builder.Services
                .AddSingleton<MainPage>()
                .AddLogging()
                .AddHttpClient()
                .AddOpenAIServices(options =>
                {
                    options.ApiKey = builder.Configuration["OpenAI:ApiKey"];
                });

            builder.Services.AddHttpClient<ITaskService, TaskService>();

//#if DEBUG
            builder.Logging.AddDebug();
//#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.Services.AddSingleton<IAuthenticator, Authenticator>();
            mauiAppBuilder.Services.AddSingleton<ITaskService, TaskService>();
            mauiAppBuilder.Services.AddSingleton<IFileService, FileService>();

            return mauiAppBuilder;
        }
    }
}