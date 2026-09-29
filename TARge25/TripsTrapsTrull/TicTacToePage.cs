namespace TARge25;

// See leht vastutab Trips-Traps-Trull mängu kasutajaliidese eest.
// Siin loome 3x3 Grid mänguvälja, nupud, pildid ning reageerime kasutaja vajutustele.
public class TicTacToePage : ContentPage
{
    // Mängu reeglid ja laua seis on eraldi klassis, et kasutada OOP põhimõtteid.
    private readonly TicTacToeGame game = new TicTacToeGame();

    // Statistika on eraldi klassis ja salvestatakse Preferences abil telefoni mällu.
    private readonly TicTacToeStatistics statistics = new TicTacToeStatistics();

    // Iga mänguruudu jaoks hoiame Image elementi, kuhu näitame X või O pilti.
    private readonly Image[,] cellImages = new Image[3, 3];

    // Random abil valime juhuslikult, kas mängu alustab X või O.
    private readonly Random random = new Random();

    // Label näitab, kelle kord on või kes võitis.
    private readonly Label statusLabel;

    // Label näitab salvestatud võitude ja viikide statistikat.
    private readonly Label statisticsLabel;

    // Siin hoiame infot, kumb mängija alustab uut mängu.
    private string startingPlayer = "X";

    public TicTacToePage()
    {
        // Lehe pealkiri NavigationPage ülemisel ribal.
        Title = "Trips-Traps-Trull";
        BackgroundColor = Colors.White;

        // Mängu suur pealkiri ekraanil.
        Label titleLabel = new Label
        {
            Text = "Trips-Traps-Trull",
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Colors.Black
        };

        // Näitab mängu hetkeolekut: kes alustab, kelle kord on või mängu tulemust.
        statusLabel = new Label
        {
            Text = "Alustab X",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#512BD4")
        };

        // Siia kirjutame X võitude, O võitude ja viikide arvu.
        statisticsLabel = new Label
        {
            FontSize = 16,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Colors.Black
        };

        // Loome mängu juhtimiseks vajalikud nupud.
        Button newGameButton = CreateButton("UUS MÄNG");
        Button starterButton = CreateButton("KES ALUSTAB?");
        Button rulesButton = CreateButton("REEGLID");
        Button resetStatisticsButton = CreateButton("NULLI STATISTIKA");

        // UUS MÄNG puhastab laua ja alustab sama algmängijaga uut mängu.
        newGameButton.Clicked += (sender, e) => StartNewGame();

        // KES ALUSTAB? valib juhuslikult X või O.
        starterButton.Clicked += OnChooseStarterClicked;

        // REEGLID avab eraldi lehe, sest ülesandes on nõutud mitmeleheküljeline lahendus.
        rulesButton.Clicked += async (sender, e) =>
            await Navigation.PushAsync(new TicTacToeRulesPage());

        // NULLI STATISTIKA kustutab salvestatud mängutulemused.
        resetStatisticsButton.Clicked += OnResetStatisticsClicked;

        // Ülemine Grid paigutab kaks nuppu kõrvuti.
        Grid buttonGrid = new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };
        buttonGrid.Add(newGameButton, 0, 0);
        buttonGrid.Add(starterButton, 1, 0);

        // Loome 3x3 mänguvälja eraldi meetodis.
        Grid boardGrid = CreateBoardGrid();

        // Alumine Grid paigutab REEGLID ja NULLI STATISTIKA nupud kõrvuti.
        Grid bottomButtons = new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };
        bottomButtons.Add(rulesButton, 0, 0);
        bottomButtons.Add(resetStatisticsButton, 1, 0);

        // ScrollView lubab sisu kerida väiksema ekraaniga telefonis.
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(20, 16, 20, 28),
                Spacing = 16,
                Children =
                {
                    titleLabel,
                    statusLabel,
                    buttonGrid,
                    boardGrid,
                    statisticsLabel,
                    bottomButtons
                }
            }
        };

        // Seadistame mängu algseisu: mängu alustab X.
        game.Reset(startingPlayer);

        // Loeme Preferences mälust varasema statistika ja näitame seda ekraanil.
        UpdateStatisticsLabel();
    }

    // Loob ülesandes nõutud 3x3 Grid mänguvälja.
    private Grid CreateBoardGrid()
    {
        Grid grid = new Grid
        {
            WidthRequest = 330,
            HeightRequest = 330,
            HorizontalOptions = LayoutOptions.Center,
            RowSpacing = 4,
            ColumnSpacing = 4,
            // Tume taust paistab valgete ruutude vahel mänguvälja joontena.
            BackgroundColor = Color.FromArgb("#333333")
        };

        // Lisame Gridile kolm võrdse kõrgusega rida ja kolm võrdse laiusega veergu.
        for (int i = 0; i < 3; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        // Loome kõik üheksa klikitavat mänguruutu.
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                // Salvestame rea ja veeru eraldi muutujatesse, et vajutus teaks õiget ruutu.
                int capturedRow = row;
                int capturedColumn = column;

                // Image näitab ruudus X või O SVG pilti.
                Image image = new Image
                {
                    Aspect = Aspect.AspectFit,
                    Margin = 18,
                    // Vajutuse võtab vastu ruutu ümbritsev Grid, mitte Image ise.
                    InputTransparent = true
                };

                // Salvestame Image viite massiivi, et saaksime hiljem selle Source väärtust muuta.
                cellImages[row, column] = image;

                // Iga valge Grid on üks klikitav mänguruut.
                Grid cell = new Grid
                {
                    BackgroundColor = Colors.White
                };
                cell.Children.Add(image);

                // TapGestureRecognizer muudab ruudu klikitavaks.
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += async (sender, e) =>
                    await OnCellTapped(capturedRow, capturedColumn);
                cell.GestureRecognizers.Add(tap);

                // Lisame ruudu õigesse 3x3 Gridi asukohta.
                grid.Add(cell, column, row);
            }
        }

        return grid;
    }

    // Käivitub siis, kui kasutaja vajutab mõnele mänguruudule.
    private async Task OnCellTapped(int row, int column)
    {
        // Proovime käigu teha. Kui ruut on juba täidetud või mäng läbi, lõpetame meetodi.
        if (!game.TryMakeMove(row, column))
            return;

        // CurrentPlayer on mängija, kes just selle käigu tegi.
        string player = game.CurrentPlayer;

        // Näitame ruudus vastava mängija pilti.
        // Pildid asuvad Resources/Images kaustas.
        cellImages[row, column].Source = player == "X" ? "x_mark.svg" : "o_mark.svg";

        // Kontrollime pärast iga käiku, kas tekkis võitja.
        string? winner = game.GetWinner();
        if (winner != null)
        {
            // Märgime mängu lõppenuks, et enam uusi käike teha ei saaks.
            game.FinishGame();

            // Lisame võidu statistikasse ja salvestame selle Preferences abil.
            statistics.AddWin(winner);
            UpdateStatisticsLabel();

            // Näitame võitjat ka lehel.
            statusLabel.Text = $"{winner} võitis!";

            // Küsime kasutajalt, kas ta soovib kohe uut mängu alustada.
            bool again = await DisplayAlertAsync(
                "Mäng läbi",
                $"{winner} võitis! Kas soovid veel mängida?",
                "Jah",
                "Ei");

            if (again)
                StartNewGame();

            return;
        }

        // Kui võitjat pole, kontrollime, kas kõik ruudud on täidetud ehk tekkis viik.
        if (game.IsDraw())
        {
            game.FinishGame();

            // Salvestame viigi statistikasse.
            statistics.AddDraw();
            UpdateStatisticsLabel();
            statusLabel.Text = "Viik!";

            // Pakume pärast viiki võimalust uut mängu alustada.
            bool again = await DisplayAlertAsync(
                "Mäng läbi",
                "Viik! Kas soovid veel mängida?",
                "Jah",
                "Ei");

            if (again)
                StartNewGame();

            return;
        }

        // Kui mäng jätkub, vahetame X ja O vahel mängijat.
        game.NextPlayer();
        statusLabel.Text = $"Käib {game.CurrentPlayer}";
    }

    // Puhastab mänguvälja ja alustab uut mängu.
    private void StartNewGame()
    {
        // Tühjendame mängu loogikas vana laua seisu.
        game.Reset(startingPlayer);

        // Eemaldame kõik X ja O pildid ekraanilt.
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                cellImages[row, column].Source = null;
            }
        }

        // Näitame, kumb mängija alustab.
        statusLabel.Text = $"Alustab {startingPlayer}";
    }

    // Valib juhuslikult, kas uut mängu alustab X või O.
    private async void OnChooseStarterClicked(object? sender, EventArgs e)
    {
        // random.Next(2) annab 0 või 1.
        startingPlayer = random.Next(2) == 0 ? "X" : "O";

        // Alustame kohe uut mängu valitud mängijaga.
        StartNewGame();

        // Teavitame kasutajat juhuslikult valitud alustajast.
        await DisplayAlertAsync(
            "Kes alustab?",
            $"Juhuslikult valiti mängija {startingPlayer}.",
            "OK");
    }

    // Küsib kinnitust ja kustutab kogu salvestatud statistika.
    private async void OnResetStatisticsClicked(object? sender, EventArgs e)
    {
        bool reset = await DisplayAlertAsync(
            "Nulli statistika",
            "Kas soovid kõik salvestatud tulemused kustutada?",
            "Jah",
            "Ei");

        // Kui kasutaja valib Ei, ei muuda me midagi.
        if (!reset)
            return;

        // Nullime väärtused ning uuendame ekraanil statistika teksti.
        statistics.Reset();
        UpdateStatisticsLabel();
    }

    // Koostab statistika teksti ja näitab seda Labelis.
    private void UpdateStatisticsLabel()
    {
        statisticsLabel.Text =
            $"Statistika: X võite {statistics.XWins}  |  O võite {statistics.OWins}  |  Viike {statistics.Draws}";
    }

    // Abimeetod loob kõik mängu nupud sama kujundusega.
    private static Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            FontSize = 14,
            HeightRequest = 50,
            BackgroundColor = Color.FromArgb("#D9D9D9"),
            TextColor = Colors.Black,
            CornerRadius = 8
        };
    }
}
