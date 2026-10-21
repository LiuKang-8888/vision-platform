namespace VisionPlatform.App;

using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VisionPlatform.Application;
using VisionPlatform.App.ViewModels;
using VisionPlatform.App.Views;
using VisionPlatform.Core.Configuration;
using VisionPlatform.Core.Tools;
using VisionPlatform.Infrastructure.Logging;
using VisionPlatform.Infrastructure.Tools.BuiltIn;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .UseVisionPlatformSerilog()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<PlatformOptions>(context.Configuration.GetSection(PlatformOptions.SectionName));
                services.Configure<WatchdogOptions>(context.Configuration.GetSection(WatchdogOptions.SectionName));
                services.Configure<InspectionPipelineOptions>(context.Configuration.GetSection(InspectionPipelineOptions.SectionName));
                services.AddVisionPlatformApplication();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>(sp =>
                    new MainWindow { DataContext = sp.GetRequiredService<MainViewModel>() });
            })
            .Build();

        BuiltInToolRegistration.RegisterAll(_host.Services.GetRequiredService<IToolRegistry>());

        var schema = _host.Services.GetRequiredService<IOptions<PlatformOptions>>().Value.SchemaVersion;
        if (schema != 1)
        {
            MessageBox.Show($"不支持的配置 SchemaVersion={schema}。", "配置错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        await _host.StartAsync().ConfigureAwait(true);

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(true);
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
