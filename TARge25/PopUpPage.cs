using Microsoft.Maui.Storage;

namespace TARge25;

public class PopUpPage : ContentPage
{
    private const string FirstStartKey = "EsimeneKäivitamine";

    public PopUpPage()
    {
        Title = "Pop-up aknad";
        BackgroundColor = Colors.White;

        Label titleLabel = new Label
        {
            Text = "Pop-up aknad",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.Black
        };

        Label infoLabel = new Label
        {
            Text = "Vali nupp ja proovi erinevaid MAUI dialoogiaknaid.",
            FontSize = 15,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Fill,
            TextColor = Colors.DarkSlateGray
        };

        Button alertButton = CreateButton("TEADE");
        alertButton.Clicked += AlertButton_Clicked;

        Button alertYesNoButton = CreateButton("JAH VÕI EI");
        alertYesNoButton.Clicked += AlertYesNoButton_Clicked;

        Button alertListButton = CreateButton("VALIK");
        alertListButton.Clicked += AlertListButton_Clicked;

        Button alertQuestButton = CreateButton("KÜSIMUS");
        alertQuestButton.Clicked += AlertQuestButton_Clicked;

        Button resetButton = CreateButton("NULLI SEADED (TESTIMISEKS)");
        resetButton.BackgroundColor = Colors.Red;
        resetButton.TextColor = Colors.White;
        resetButton.Margin = new Thickness(0, 20, 0, 0);
        resetButton.Clicked += ResetButton_Clicked;

        VerticalStackLayout contentLayout = new VerticalStackLayout
        {
            Spacing = 18,
            Padding = new Thickness(30, 55, 30, 30),
            HorizontalOptions = LayoutOptions.Fill,
            Children =
            {
                titleLabel,
                infoLabel,
                alertButton,
                alertYesNoButton,
                alertListButton,
                alertQuestButton,
                resetButton
            }
        };

        Content = new ScrollView
        {
            Content = contentLayout
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool onEsimeneStart = Preferences.Default.Get(FirstStartKey, true);

        if (onEsimeneStart)
        {
            bool vastus = await DisplayAlertAsync(
                "Tere tulemast!",
                "Tundub, et avasid selle rakenduse esimest korda. Kas soovid näha lühikest juhendit?",
                "Jah, palun",
                "Ei, saan ise hakkama");

            if (vastus)
            {
                await DisplayAlertAsync(
                    "Juhend",
                    "Vali menüüst sobiv nupp ja uuri, kuidas erinevad pop-up aknad töötavad.",
                    "Selge");
            }

            Preferences.Default.Set(FirstStartKey, false);
        }
    }

    private static Button CreateButton(string text)
    {
        return new Button
        {
            Text = text,
            FontSize = 16,
            HeightRequest = 55,
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = 260,
            CornerRadius = 10
        };
    }

    private async void AlertButton_Clicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("Teade", "Teil on uus teade", "OK");
    }

    private async void AlertYesNoButton_Clicked(object? sender, EventArgs e)
    {
        bool result = await DisplayAlertAsync(
            "Kinnitus",
            "Kas oled kindel?",
            "Olen kindel",
            "Ei ole kindel");

        await DisplayAlertAsync(
            "Teade",
            "Teie valik on: " + (result ? "Jah" : "Ei"),
            "OK");
    }

    private async void AlertListButton_Clicked(object? sender, EventArgs e)
    {
        string? action = await DisplayActionSheetAsync(
            "Mida teha?",
            "Loobu",
            null,
            "Tantsida",
            "Laulda",
            "Joonestada");

        if (!string.IsNullOrEmpty(action) && action != "Loobu")
        {
            await DisplayAlertAsync(
                "Valik",
                "Sa valisid tegevuse: " + action,
                "OK");
        }
    }

    private async void AlertQuestButton_Clicked(object? sender, EventArgs e)
    {
        string? result1 = await DisplayPromptAsync(
            "Küsimus",
            "Kuidas läheb?",
            placeholder: "Tore!");

        if (result1 is null)
            return;

        string? result2 = await DisplayPromptAsync(
            "Vasta",
            "Millega võrdub 5 + 5?",
            initialValue: "10",
            maxLength: 2,
            keyboard: Keyboard.Numeric);

        if (result2 is null)
            return;

        await DisplayAlertAsync(
            "Vastused",
            $"Kuidas läheb: {result1}\n5 + 5 = {result2}",
            "OK");
    }

    private async void ResetButton_Clicked(object? sender, EventArgs e)
    {
        Preferences.Default.Remove(FirstStartKey);

        await DisplayAlertAsync(
            "Edukalt nullitud",
            "Mälu on tühjendatud. Kui avad lehe või rakenduse uuesti, käitub see nagu esimesel käivitamisel.",
            "OK");
    }
}
