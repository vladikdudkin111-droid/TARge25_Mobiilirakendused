using Microsoft.Maui.Storage;

namespace TARge25;

// See klass vastutab mängu tulemuste salvestamise eest.
// Preferences võimaldab statistika alles hoida ka pärast rakenduse sulgemist.
public class TicTacToeStatistics
{
    // Need võtmed on nimed, mille alla salvestame väärtused telefoni mällu.
    private const string XWinsKey = "TTT_XWins";
    private const string OWinsKey = "TTT_OWins";
    private const string DrawsKey = "TTT_Draws";

    // Avalikud omadused annavad lehele statistika lugemiseks väärtused.
    // private set tähendab, et muuta saab neid ainult selle klassi sees.
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    public TicTacToeStatistics()
    {
        // Klassi loomisel loeme varem salvestatud statistika kohe mälust.
        Load();
    }

    // Lisab võidu õigele mängijale ja salvestab tulemuse.
    public void AddWin(string player)
    {
        if (player == "X")
            XWins++;
        else if (player == "O")
            OWins++;

        Save();
    }

    // Lisab ühe viigi ja salvestab uue väärtuse.
    public void AddDraw()
    {
        Draws++;
        Save();
    }

    // Nullib kogu statistika.
    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;

        // Salvestame nullitud väärtused ka Preferences mällu.
        Save();
    }

    // Loeb Preferences mälust varem salvestatud väärtused.
    private void Load()
    {
        // Kui väärtust pole veel olemas, kasutatakse vaikimisi 0.
        XWins = Preferences.Default.Get(XWinsKey, 0);
        OWins = Preferences.Default.Get(OWinsKey, 0);
        Draws = Preferences.Default.Get(DrawsKey, 0);
    }

    // Salvestab kõik kolm statistika väärtust Preferences mällu.
    private void Save()
    {
        Preferences.Default.Set(XWinsKey, XWins);
        Preferences.Default.Set(OWinsKey, OWins);
        Preferences.Default.Set(DrawsKey, Draws);
    }
}
