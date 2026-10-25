using System.ComponentModel;
using Heliaed.Models;

namespace Heliaed.Services;

// Teema rakendub kogu rakendusele; värvid määratakse AppThemeBinding abil.
public sealed class ThemeService
{
    private readonly AppSettings settings;
    public ThemeService(SettingsService service)
    {
        settings = service.Values;
        settings.PropertyChanged += OnSettingsChanged;
    }
    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.IsDark)) Apply();
    }
    public void Apply()
    {
        if (Application.Current is { } app)
            app.UserAppTheme = settings.IsDark ? AppTheme.Dark : AppTheme.Light;
    }
}
