using Heliaed.Services;
using Heliaed.ViewModels;
using Heliaed.Views;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace Heliaed;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddSingleton<ISettingsStore, PreferencesSettingsStore>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<ThemeService>();
        builder.Services.AddSingleton<SceneRepository>(_ =>
            new SceneRepository(Path.Combine(FileSystem.AppDataDirectory, "heliaed.db3")));
        builder.Services.AddSingleton<ISceneRepository>(sp => sp.GetRequiredService<SceneRepository>());
        builder.Services.AddSingleton<IAudioManager>(_ => AudioManager.Current);
        builder.Services.AddSingleton<AudioService>();
        builder.Services.AddSingleton<IAudioService>(sp => sp.GetRequiredService<AudioService>());
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainTabs>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
