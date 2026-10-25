namespace Heliaed.Services;

public sealed class PreferencesSettingsStore : ISettingsStore
{
    private const string Prefix = "heliaed.";
    public string Get(string key, string fallback) => Preferences.Default.Get(Prefix + key, fallback);
    public void Set(string key, string value) => Preferences.Default.Set(Prefix + key, value);
}
