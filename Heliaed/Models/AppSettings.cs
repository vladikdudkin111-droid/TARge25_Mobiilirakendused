using Heliaed.ViewModels;

namespace Heliaed.Models;

public sealed class AppSettings : ObservableObject
{
    private string language = "et", selectedKind = "leaf";
    private bool isDark, soundEnabled = true, musicPlaying;
    private double volume = 0.35;
    public string Language { get => language; set => Set(ref language, value == "en" ? "en" : "et"); }
    public bool IsDark { get => isDark; set => Set(ref isDark, value); }
    public bool SoundEnabled { get => soundEnabled; set => Set(ref soundEnabled, value); }
    public bool MusicPlaying { get => musicPlaying; set => Set(ref musicPlaying, value); }
    public double Volume { get => volume; set => Set(ref volume, double.IsFinite(value) ? Math.Clamp(value, 0, 1) : 0.35); }
    public string SelectedKind
    {
        get => selectedKind;
        set
        {
            if (value == "fish") value = "house";
            Set(ref selectedKind, GardenLayout.Kinds.Contains(value) ? value : "leaf");
        }
    }
}
