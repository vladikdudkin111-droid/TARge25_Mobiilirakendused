namespace TARge25.MemoryGame;

// Theme hoiab ühes kohas taustavärve, teksti värve ja kasutatavat fonti.
public sealed class Theme
{
    public string Name { get; }
    public Color BackgroundColor { get; }
    public Color PanelColor { get; }
    public Color TextColor { get; }
    public Color MutedTextColor { get; }
    public Color AccentColor { get; }
    public Color CardBackColor { get; }
    public Color CardFaceColor { get; }
    public Color MatchedColor { get; }
    public string FontFamily { get; }

    // Konstruktoriga määrame konkreetse teema kõik kujundusväärtused.
    public Theme(string name, Color backgroundColor, Color panelColor, Color textColor,
        Color mutedTextColor, Color accentColor, Color cardBackColor, Color cardFaceColor,
        Color matchedColor, string fontFamily)
    {
        Name = name;
        BackgroundColor = backgroundColor;
        PanelColor = panelColor;
        TextColor = textColor;
        MutedTextColor = mutedTextColor;
        AccentColor = accentColor;
        CardBackColor = cardBackColor;
        CardFaceColor = cardFaceColor;
        MatchedColor = matchedColor;
        FontFamily = fontFamily;
    }

    // Loome kolm eraldi teemat: hele, tume ja värviline.
    public static IReadOnlyList<Theme> All { get; } = new List<Theme>
    {
        new("Hele", Color.FromArgb("#F2F6FF"), Colors.White,
            Color.FromArgb("#152448"), Color.FromArgb("#677596"),
            Color.FromArgb("#4263EB"), Color.FromArgb("#5B72D6"),
            Color.FromArgb("#E4EDFF"), Color.FromArgb("#B8F0D4"), "OpenSansRegular"),
        new("Tume", Color.FromArgb("#0C132B"), Color.FromArgb("#1A2544"),
            Colors.White, Color.FromArgb("#B2C0E2"),
            Color.FromArgb("#8058E8"), Color.FromArgb("#344579"),
            Color.FromArgb("#2E3F69"), Color.FromArgb("#245A55"), "OpenSansSemibold"),
        new("Värviline", Color.FromArgb("#FFF1F9"), Color.FromArgb("#FFFFFF"),
            Color.FromArgb("#66306B"), Color.FromArgb("#9B6389"),
            Color.FromArgb("#E659AE"), Color.FromArgb("#E775B9"),
            Color.FromArgb("#FCE1F4"), Color.FromArgb("#FFE29B"), "OpenSansSemibold")
    };

    // Rakendab teema ContentPage taustale ja XAML-i DynamicResource väärtustele.
    // Tänu DynamicResource'ile muutuvad tekstivärvid ja fondid kohe, ilma lehte uuesti avamata.
    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;
        page.Resources["MemoBackground"] = BackgroundColor;
        page.Resources["MemoPanel"] = PanelColor;
        page.Resources["MemoText"] = TextColor;
        page.Resources["MemoMutedText"] = MutedTextColor;
        page.Resources["MemoAccent"] = AccentColor;
        page.Resources["MemoFont"] = FontFamily;
    }
}
