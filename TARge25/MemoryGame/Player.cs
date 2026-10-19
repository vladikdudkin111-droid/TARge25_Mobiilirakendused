using Microsoft.Maui.Storage;

namespace TARge25.MemoryGame;

// Player hoiab mängija nime ja salvestatud tulemuste statistikat.
public sealed class Player
{
    private const string NameKey = "TARge25_Memo_PlayerName";
    private const string WinsKey = "TARge25_Memo_Wins";
    private const string BestScoreKey = "TARge25_Memo_BestScore";
    private const string BestTimeKey = "TARge25_Memo_BestTime";

    public string Name { get; private set; }
    public int Wins { get; private set; }
    public int BestScore { get; private set; }
    public int BestTimeSeconds { get; private set; }

    // Konstruktor laeb eelmised tulemused Preferences mälust.
    public Player()
    {
        Name = Preferences.Default.Get(NameKey, "Mängija");
        Wins = Preferences.Default.Get(WinsKey, 0);
        BestScore = Preferences.Default.Get(BestScoreKey, 0);
        BestTimeSeconds = Preferences.Default.Get(BestTimeKey, 0);
    }

    // Mängija nime saab muuta ning uus nimi jääb järgmisteks käivitusteks alles.
    public void SetName(string? name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Mängija" : name.Trim();
        Preferences.Default.Set(NameKey, Name);
    }

    // Salvestame võidu ja vajadusel parandame isiklikku rekordit.
    public void RecordWin(int score, int elapsedSeconds)
    {
        Wins++;
        BestScore = Math.Max(BestScore, score);
        if (BestTimeSeconds == 0 || elapsedSeconds < BestTimeSeconds)
        {
            BestTimeSeconds = elapsedSeconds;
        }

        Preferences.Default.Set(WinsKey, Wins);
        Preferences.Default.Set(BestScoreKey, BestScore);
        Preferences.Default.Set(BestTimeKey, BestTimeSeconds);
    }
}
