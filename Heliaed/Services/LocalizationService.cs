using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Heliaed.Services;

public sealed class LocalizationService : INotifyPropertyChanged
{
    public static LocalizationService Current { get; } = new();
    private readonly ResourceManager resources = new("Heliaed.Resources.Localization.AppResources", typeof(LocalizationService).Assembly);
    private CultureInfo culture = CultureInfo.GetCultureInfo("et");
    public event PropertyChangedEventHandler? PropertyChanged;
    public string this[string key] => resources.GetString(key, culture) ?? key;
    public string Language => culture.TwoLetterISOLanguageName;
    public void SetLanguage(string language)
    {
        culture = CultureInfo.GetCultureInfo(language == "en" ? "en" : "et");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        // Tühi nimi värskendab korraga kõiki indekseri sidumisi.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}
