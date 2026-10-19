using Microsoft.Maui.Dispatching;

namespace TARge25.MemoryGame;

// MemoryGamePage seob XAML kasutajaliidese OOP mänguklassidega.
public partial class MemoryGamePage : ContentPage
{
    // Game juhib mängureegleid, Player hoiab statistikat ja Theme kujundust.
    private readonly Game game = new();
    private readonly Player player = new();
    private Theme currentTheme = Theme.All[0];

    // IDispatcherTimer uuendab mängu aega iga sekundi järel.
    private IDispatcherTimer? gameTimer;

    public MemoryGamePage()
    {
        InitializeComponent();

        // Täidame nimevälja varem salvestatud mängija nimega.
        PlayerNameEntry.Text = player.Name;

        // Seome mängu kaardid XAML-is loodud CollectionView-ga.
        CardsCollection.ItemsSource = game.Cards;

        // Käivitamisel valime heleda teema ja loome mänguvälja.
        ThemePicker.SelectedIndex = 0;
        StartNewGame();
    }

    // Muudab teemat kohe, kui Pickeri valik muutub.
    private void OnThemeChanged(object sender, EventArgs e)
    {
        int index = ThemePicker.SelectedIndex;
        if (index < 0 || index >= Theme.All.Count) return;

        currentTheme = Theme.All[index];
        currentTheme.Apply(this);

        // Kaartide värve tuleb samuti uuendada, sest need sõltuvad kaardi olekust.
        foreach (Card card in game.Cards)
        {
            card.ApplyTheme(currentTheme);
        }
    }

    // Nupp Uus mäng nullib varasema mängu ja segab kaardid uuesti.
    private void OnStartClicked(object sender, EventArgs e)
    {
        StartNewGame();
    }

    // Teeb päriselt uue mänguringi, aga jätab salvestatud rekordid alles.
    private void StartNewGame()
    {
        player.SetName(PlayerNameEntry.Text);
        game.Start();

        // Rakendame aktiivse teema kõigile uutele kaartidele.
        foreach (Card card in game.Cards)
        {
            card.ApplyTheme(currentTheme);
        }

        PauseOverlay.IsVisible = false;
        PauseButton.IsEnabled = true;
        PauseButton.Text = "⏸ Paus";
        StatusLabel.Text = "Leia kaks ühesugust kaarti!";
        RefreshStats();
        StartTimer();
    }

    // Nupp Paus peatab või jätkab mängu, kui kasutaja seda vajutab.
    private void OnPauseClicked(object sender, EventArgs e)
    {
        game.TogglePause();
        PauseOverlay.IsVisible = game.IsPaused;
        PauseButton.Text = game.IsPaused ? "▶ Jätka" : "⏸ Paus";
        StatusLabel.Text = game.IsPaused ? "Mäng on pausil." : "Mäng jätkub!";
    }

    // Kasutaja vajutab kaardile: avame selle ja küsime tulemuse Game klassilt.
    private async void OnCardTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not Card card) return;

        // Märgime mänguringi numbri, et vana viivitusega tegevus ei mõjutaks uut mängu.
        int roundNumber = game.RoundNumber;
        MoveResult result = game.FlipCard(card);
        RefreshStats();

        switch (result)
        {
            case MoveResult.FirstCard:
                StatusLabel.Text = "Vali teine kaart!";
                break;

            case MoveResult.Match:
                StatusLabel.Text = "Tubli! Leidsid paari! 🎉";
                await AnimateStatusAsync();
                break;

            case MoveResult.Mismatch:
                StatusLabel.Text = "See paar ei sobi. Proovi uuesti!";
                // Näitame mõlemat sümbolit enne, kui kaardid tagasi pöörame.
                await Task.Delay(950);
                if (roundNumber == game.RoundNumber)
                {
                    game.HideMismatch();
                }
                break;

            case MoveResult.Won:
                // Viimane paar lõpetab mängu ja salvestab tulemuse Preferences mällu.
                gameTimer?.Stop();
                player.RecordWin(game.Score, game.ElapsedSeconds);
                StatusLabel.Text = "Palju õnne! Kõik paarid on leitud! 🏆";
                PauseButton.IsEnabled = false;
                RefreshStats();
                await AnimateStatusAsync();
                bool playAgain = await DisplayAlertAsync(
                    "Võit! 🏆",
                    $"{player.Name}, leidsid kõik {game.TotalPairs} paari!\n" +
                    $"Käigud: {game.Attempts}\nAeg: {FormatTime(game.ElapsedSeconds)}\n" +
                    $"Punkte: {game.Score}\n\nKas soovid uuesti mängida?",
                    "Uus mäng", "Sulge");
                if (playAgain) StartNewGame();
                break;
        }
    }

    // Väike animatsioon suurendab ja taastab staatuse teksti.
    private async Task AnimateStatusAsync()
    {
        await StatusLabel.ScaleToAsync(1.08, 120);
        await StatusLabel.ScaleToAsync(1.0, 120);
    }

    // Värskendab paaride arvu, käigud, punktid, aja ja rekordid.
    private void RefreshStats()
    {
        PairsLabel.Text = $"{game.PairsFound} / {game.TotalPairs}";
        AttemptsLabel.Text = game.Attempts.ToString();
        ScoreLabel.Text = game.Score.ToString();
        TimeLabel.Text = FormatTime(game.ElapsedSeconds);
        WinsLabel.Text = $"Võite: {player.Wins}";
        BestScoreLabel.Text = $"Parim tulemus: {player.BestScore}";
        BestTimeLabel.Text = player.BestTimeSeconds > 0
            ? $"Kiireim võit: {FormatTime(player.BestTimeSeconds)}"
            : "Kiireim võit: –";
    }

    // Kuvame aja kujul minutid:sekundid.
    private static string FormatTime(int seconds)
    {
        return $"{seconds / 60:00}:{seconds % 60:00}";
    }

    // Loome ajaloenduri ainult üks kord ja jätkame selle tööd vajadusel.
    private void StartTimer()
    {
        if (gameTimer == null)
        {
            gameTimer = Dispatcher.CreateTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += (_, _) =>
            {
                game.Tick();
                RefreshStats();
                if (!game.IsRunning) gameTimer.Stop();
            };
        }

        if (game.IsRunning && !gameTimer.IsRunning)
        {
            gameTimer.Start();
        }
    }

    // Kui leht muutub nähtavaks, jätkame pooleli olevat mängu.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartTimer();
    }

    // Lehelt lahkudes peatame taimeri, et see ei töötaks taustal.
    protected override void OnDisappearing()
    {
        gameTimer?.Stop();
        base.OnDisappearing();
    }
}
