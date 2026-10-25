using System.Globalization;
using Heliaed.Models;

namespace Heliaed.Services;

// Seaded taastatakse enne vaadete loomist ja salvestatakse iga muutuse järel.
public sealed class SettingsService
{
    public AppSettings Values { get; }
    public SettingsService(ISettingsStore store)
    {
        Values = new AppSettings
        {
            Language = store.Get("language", "et"),
            IsDark = store.Get("dark", "False") == "True",
            SoundEnabled = store.Get("sound", "True") == "True",
            MusicPlaying = store.Get("playing", "False") == "True",
            Volume = double.TryParse(store.Get("volume", "0.35"), CultureInfo.InvariantCulture, out var v) ? v : 0.35,
            SelectedKind = store.Get("kind", "leaf")
        };
        Values.PropertyChanged += (_, _) =>
        {
            store.Set("language", Values.Language);
            store.Set("dark", Values.IsDark.ToString());
            store.Set("sound", Values.SoundEnabled.ToString());
            store.Set("playing", Values.MusicPlaying.ToString());
            store.Set("volume", Values.Volume.ToString(CultureInfo.InvariantCulture));
            store.Set("kind", Values.SelectedKind);
        };
    }
}
