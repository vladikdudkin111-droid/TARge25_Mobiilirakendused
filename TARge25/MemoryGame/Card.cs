using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TARge25.MemoryGame;

// Card kirjeldab ühte Memo mängu kaarti ja selle olekut.
public sealed class Card : INotifyPropertyChanged
{
    // Kaardi paari number määrab, millised kaks kaarti kokku sobivad.
    public int PairId { get; }

    // Sümbol kuvatakse siis, kui kaart on avatud või paar on leitud.
    public string Symbol { get; }

    // Need omadused on muudetavad ainult kaardi enda meetodite kaudu.
    public bool IsFaceUp { get; private set; }
    public bool IsMatched { get; private set; }

    // Kaardi välimust värskendatakse aktiivse teema järgi.
    private Color hiddenColor = Color.FromArgb("#5575C9");
    private Color faceColor = Colors.White;
    private Color matchedColor = Color.FromArgb("#BBF7D0");
    private Color textColor = Color.FromArgb("#1C2862");

    public event PropertyChangedEventHandler? PropertyChanged;

    // UI saab DisplaySymbol omaduse kaudu näidata kas küsimärki või sümbolit.
    public string DisplaySymbol => IsFaceUp || IsMatched ? Symbol : "?";

    public Color TileBackground => IsMatched ? matchedColor : IsFaceUp ? faceColor : hiddenColor;
    public Color SymbolColor => IsFaceUp || IsMatched ? textColor : Colors.White;

    public Card(int pairId, string symbol)
    {
        PairId = pairId;
        Symbol = symbol;
    }

    // Avab kaardi, et mängija näeks peidetud sümbolit.
    public void Reveal()
    {
        if (IsFaceUp || IsMatched) return;
        IsFaceUp = true;
        NotifyVisualChanged();
    }

    // Sulgeb vale paari kaardi uuesti.
    public void Hide()
    {
        if (IsMatched) return;
        IsFaceUp = false;
        NotifyVisualChanged();
    }

    // Märgib õige paari leitud kaartideks, mida ei saa enam sulgeda.
    public void MarkMatched()
    {
        IsMatched = true;
        IsFaceUp = true;
        NotifyVisualChanged();
    }

    // Uue teema valimisel värskendab iga kaart oma värve.
    public void ApplyTheme(Theme theme)
    {
        hiddenColor = theme.CardBackColor;
        faceColor = theme.CardFaceColor;
        matchedColor = theme.MatchedColor;
        textColor = theme.TextColor;
        NotifyVisualChanged();
    }

    // Teavitab XAML-i Bindingut, et muutunud andmed tuleb ekraanile joonistada.
    private void NotifyVisualChanged()
    {
        OnPropertyChanged(nameof(DisplaySymbol));
        OnPropertyChanged(nameof(TileBackground));
        OnPropertyChanged(nameof(SymbolColor));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
