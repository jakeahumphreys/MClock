using System;
using System.IO;
using System.Windows;
using MClock.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MClock
{
    public partial class App : Application
    {
        private IServiceProvider? ServiceProvider { get; set; }
        private IConfiguration? Configuration { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();

            var appSettingsService = new AppSettingsService(Configuration);

            serviceCollection.AddSingleton(appSettingsService.GetSettings());
            serviceCollection.AddSingleton(Configuration);
            serviceCollection.AddSingleton(typeof(MainWindow));
            
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
