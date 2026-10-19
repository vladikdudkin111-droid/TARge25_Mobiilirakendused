using System.Collections.ObjectModel;

namespace TARge25.MemoryGame;

// Mängu käigu tulemused: esimene kaart, leitud paar, vale paar või võit.
public enum MoveResult
{
    Ignored,
    FirstCard,
    Match,
    Mismatch,
    Won
}

// Game sisaldab Memo mängu reegleid. UI üksnes näitab mängu olekut.
public sealed class Game
{
    private static readonly string[] Symbols =
    {
        "🐶", "🐱", "🦊", "🐻", "🐼", "🐸", "🐵", "🐯"
    };

    // ObservableCollection annab CollectionView-le teada kaardiloendi muutumisest.
    public ObservableCollection<Card> Cards { get; } = new();

    // Neid väärtusi saab muuta ainult Game klassi meetoditest.
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsLocked { get; private set; }
    public int Attempts { get; private set; }
    public int PairsFound { get; private set; }
    public int ElapsedSeconds { get; private set; }
    public int RoundNumber { get; private set; }
    public int TotalPairs => Symbols.Length;

    // Punktid sõltuvad leitud paaridest, käikudest ja mänguajast.
    public int Score => Math.Max(0, PairsFound * 100 - Math.Max(0, Attempts - PairsFound) * 10 - ElapsedSeconds);

    private Card? firstCard;
    private Card? secondCard;

    // Game konstruktor loob mängu oleku; kaardid lisatakse Start meetodis.
    public Game()
    {
        IsRunning = false;
    }

    // Start segab sümbolid ja alustab uut mängu.
    public void Start()
    {
        RoundNumber++;
        Cards.Clear();
        Attempts = 0;
        PairsFound = 0;
        ElapsedSeconds = 0;
        firstCard = null;
        secondCard = null;
        IsLocked = false;
        IsPaused = false;
        IsRunning = true;

        // Iga sümbol lisatakse kaks korda, et saaks moodustada paare.
        var newCards = Symbols.SelectMany((symbol, index) => new[]
        {
            new Card(index, symbol),
            new Card(index, symbol)
        }).ToList();

        // Fisher-Yates algoritm segab kaardid juhuslikku järjekorda.
        for (int i = newCards.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (newCards[i], newCards[j]) = (newCards[j], newCards[i]);
        }

        foreach (Card card in newCards)
        {
            Cards.Add(card);
        }
    }

    // Paus peatab mängija tegevused ja ajaloenduri.
    public void TogglePause()
    {
        if (IsRunning) IsPaused = !IsPaused;
    }

    // Stop lõpetab käimasoleva mängu (näiteks lehelt lahkudes).
    public void Stop()
    {
        IsRunning = false;
        IsPaused = false;
        IsLocked = false;
    }

    // Taimer käivitab selle meetodi iga sekundi järel.
    public void Tick()
    {
        if (IsRunning && !IsPaused) ElapsedSeconds++;
    }

    // FlipCard kontrollib, kas kaarti tohib avada, ning võrdleb kahte valikut.
    public MoveResult FlipCard(Card card)
    {
        if (!IsRunning || IsPaused || IsLocked || card.IsFaceUp || card.IsMatched || !Cards.Contains(card))
            return MoveResult.Ignored;

        card.Reveal();
        if (firstCard == null)
        {
            firstCard = card;
            return MoveResult.FirstCard;
        }

        secondCard = card;
        Attempts++;

        if (firstCard.PairId == secondCard.PairId)
        {
            firstCard.MarkMatched();
            secondCard.MarkMatched();
            firstCard = null;
            secondCard = null;
            PairsFound++;

            // Kui kõik paarid on leitud, on mäng võidetud.
            if (PairsFound == TotalPairs)
            {
                IsRunning = false;
                return MoveResult.Won;
            }

            return MoveResult.Match;
        }

        // Vale paari puhul lukustame hetkeks kaardid, et oodata animatsiooni.
        IsLocked = true;
        return MoveResult.Mismatch;
    }

    // HideMismatch keerab kaks valet kaarti tagasi, et mäng võiks jätkuda.
    public void HideMismatch()
    {
        if (!IsLocked) return;
        firstCard?.Hide();
        secondCard?.Hide();
        firstCard = null;
        secondCard = null;
        IsLocked = false;
    }
}
