namespace TARge25;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        // Muudan nupu teksti vastavalt vajutuste arvule.
        if (count == 1)
            CounterBtn.Text = $"Vajutatud {count} kord";
        else
            CounterBtn.Text = $"Vajutatud {count} korda";

        // Pööran pilti iga vajutusega 15 kraadi.
        BotImage.Rotation += 15;

        // Muudan loenduri kirjeldust.
        CounterLabel.Text = $"Nuppu on vajutatud kokku: {count}";

        // Genereerin juhusliku värvi Reset nupule.
        var random = new Random();
        var randomColor = Color.FromRgb(
            random.Next(0, 256),
            random.Next(0, 256),
            random.Next(0, 256));

        ResetBtn.BackgroundColor = randomColor;

        // Kui loendur jõuab kümneni, peidan pildi.
        if (count >= 10)
        {
            BotImage.IsVisible = false;
            CounterLabel.Text = "Pilt kadus ära! Vajuta Reset.";
        }

        // Kui loendur jõuab viieni, muutub Counter nupp punaseks.
        if (count >= 5)
        {
            CounterBtn.BackgroundColor = Colors.Red;
            CounterBtn.TextColor = Colors.White;
        }

        // Muudan pildi suurust iga vajutusega suuremaks.
        BotImage.Scale += 0.1;

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        // Nullin loenduri.
        count = 0;

        // Taastan algse teksti.
        CounterBtn.Text = "Vajuta mind";
        CounterLabel.Text = "Alustame uuesti!";

        // Taastan pildi algse pöördenurga.
        BotImage.Rotation = 0;

        // Toon peidetud pildi tagasi.
        BotImage.IsVisible = true;

        // Eemaldan Reset nupu muudetud taustavärvi.
        ResetBtn.ClearValue(BackgroundColorProperty);

        // Eemaldan Counter nupu muudetud taustavärvi.
        CounterBtn.ClearValue(BackgroundColorProperty);

        // Eemaldan Counter nupu muudetud tekstivärvi.
        CounterBtn.ClearValue(Button.TextColorProperty);

        // Liigutan pilti vasaku ja parema serva vahel.
        if (BotImage.HorizontalOptions == LayoutOptions.Start)
        {
            BotImage.HorizontalOptions = LayoutOptions.End;
        }
        else
        {
            BotImage.HorizontalOptions = LayoutOptions.Start;
        }

        // Taastan pildi algse suuruse.
        BotImage.Scale = 1;
    }
}
