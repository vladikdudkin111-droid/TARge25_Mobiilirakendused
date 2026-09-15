namespace TARge25;

public partial class TreePage : ContentPage
{
    private readonly Random random = new();
    private Color normalLeafColor = Color.FromArgb("#49A942");
    private Color normalTopLeafColor = Color.FromArgb("#55B94D");

    public TreePage()
    {
        InitializeComponent();
        UpdateSkyForMonth(SeasonDatePicker.Date?.Month ?? DateTime.Today.Month);
    }

    private async void OnActionClicked(object? sender, EventArgs e)
    {
        if (ActionPicker.SelectedItem is not string action)
        {
            InfoLabel.Text = "Vali kõigepealt tegevus!";
            return;
        }

        switch (action)
        {
            case "Kasva":
                await GrowTreeAsync();
                break;

            case "Õitse":
                await BloomTreeAsync();
                break;

            case "Värise":
                await ShakeTreeAsync();
                break;

            case "Langeta":
                await FellTreeAsync();
                break;
        }
    }

    private async Task GrowTreeAsync()
    {
        ResetTreePosition();

        // Loovuse lisa: iga kord kasvab puu juhuslikult erineva suurusega.
        double targetScale = 1.2 + random.NextDouble() * 0.4;
        InfoLabel.Text = $"Puu kasvab! Suurus: {targetScale:F2}x";

        await TreeGroup.ScaleTo(targetScale, GetDuration());
    }

    private async Task BloomTreeAsync()
    {
        ResetTreePosition();
        TreeGroup.IsVisible = true;
        TreeGroup.Opacity = 1;

        CrownLeft.BackgroundColor = Colors.LightPink;
        CrownRight.BackgroundColor = Colors.Pink;
        CrownTop.BackgroundColor = Colors.LightPink;

        Flower1.IsVisible = true;
        Flower2.IsVisible = true;
        Flower3.IsVisible = true;
        Flower4.IsVisible = true;

        RestoreApples();
        InfoLabel.Text = "Puu õitseb!";

        await Task.WhenAll(
            CrownLeft.ScaleTo(1.08, GetDuration() / 2),
            CrownRight.ScaleTo(1.08, GetDuration() / 2),
            CrownTop.ScaleTo(1.08, GetDuration() / 2)
        );

        await Task.WhenAll(
            CrownLeft.ScaleTo(1.0, GetDuration() / 2),
            CrownRight.ScaleTo(1.0, GetDuration() / 2),
            CrownTop.ScaleTo(1.0, GetDuration() / 2)
        );
    }

    private async Task ShakeTreeAsync()
    {
        ResetTreePosition();
        InfoLabel.Text = "Puu väriseb tuules!";

        uint part = Math.Max(100u, GetDuration() / 4);
        await TreeGroup.TranslateTo(-25, 0, part);
        await TreeGroup.TranslateTo(25, 0, part);
        await TreeGroup.TranslateTo(-18, 0, part);
        await TreeGroup.TranslateTo(18, 0, part);
        await TreeGroup.TranslateTo(0, 0, part);
    }

    private async Task FellTreeAsync()
    {
        int month = SeasonDatePicker.Date?.Month ?? DateTime.Today.Month;
        TimeSpan time = WorkTimePicker.Time ?? TimeSpan.FromHours(12);

        bool isWinter = month == 12 || month == 1 || month == 2;
        bool isDaylight = time >= TimeSpan.FromHours(8) && time <= TimeSpan.FromHours(17);

        if (!isWinter || !isDaylight)
        {
            if (!isWinter && !isDaylight)
                InfoLabel.Text = "Pimedas ja väljaspool talve puid ei langetata!";
            else if (!isWinter)
                InfoLabel.Text = "Puud tohib langetada ainult talvel (12, 1 või 2)!";
            else
                InfoLabel.Text = "Puud tohib langetada ainult valgel ajal 08:00–17:00!";

            return;
        }

        TreeGroup.IsVisible = true;
        TreeGroup.Opacity = 1;
        TreeGroup.TranslationX = 0;
        TreeGroup.TranslationY = 0;
        InfoLabel.Text = "Tingimused sobivad. Puu langeb!";

        await TreeGroup.RotateTo(90, GetDuration());
    }

    private void OnOpacityChanged(object? sender, ValueChangedEventArgs e)
    {
        double opacity = e.NewValue;

        CrownLeft.Opacity = opacity;
        CrownRight.Opacity = opacity;
        CrownTop.Opacity = opacity;
        Flower1.Opacity = opacity;
        Flower2.Opacity = opacity;
        Flower3.Opacity = opacity;
        Flower4.Opacity = opacity;
        Apple1.Opacity = opacity;
        Apple2.Opacity = opacity;
        Apple3.Opacity = opacity;

        OpacityLabel.Text = $"Lehestiku läbipaistvus: {(int)(opacity * 100)}%";
    }

    private void OnSpeedChanged(object? sender, ValueChangedEventArgs e)
    {
        SpeedLabel.Text = $"Animatsiooni aeg: {(int)e.NewValue} ms";
    }

    private void OnDateSelected(object? sender, DateChangedEventArgs e)
    {
        UpdateSkyForMonth(e.NewDate?.Month ?? DateTime.Today.Month);
    }

    private void UpdateSkyForMonth(int month)
    {
        // Loovuse lisa: taevas muutub DatePickeris valitud aastaaja järgi.
        if (month is 6 or 7 or 8)
        {
            Scene.BackgroundColor = Color.FromArgb("#9DDBFF");
            SunMoonLabel.Text = "☀️";
        }
        else if (month is 9 or 10 or 11)
        {
            Scene.BackgroundColor = Color.FromArgb("#F2BE7E");
            SunMoonLabel.Text = "🌤️";
        }
        else if (month is 12 or 1 or 2)
        {
            Scene.BackgroundColor = Color.FromArgb("#C7D0D9");
            SunMoonLabel.Text = "❄️";
        }
        else
        {
            Scene.BackgroundColor = Color.FromArgb("#CFF0C8");
            SunMoonLabel.Text = "🌤️";
        }
    }

    private async void OnAppleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not string appleName)
            return;

        Frame? apple = appleName switch
        {
            "Apple1" => Apple1,
            "Apple2" => Apple2,
            "Apple3" => Apple3,
            _ => null
        };

        if (apple is null || !apple.IsVisible)
            return;

        InfoLabel.Text = "Õun kukub puu otsast!";

        await Task.WhenAll(
            apple.TranslateTo(0, 190, GetDuration()),
            apple.FadeTo(0, GetDuration())
        );

        apple.IsVisible = false;
    }

    private void RestoreApples()
    {
        foreach (Frame apple in new[] { Apple1, Apple2, Apple3 })
        {
            apple.IsVisible = true;
            apple.TranslationX = 0;
            apple.TranslationY = 0;
            apple.Opacity = OpacitySlider.Value;
        }
    }

    private void ResetTreePosition()
    {
        TreeGroup.IsVisible = true;
        TreeGroup.Opacity = 1;
        TreeGroup.Rotation = 0;
        TreeGroup.TranslationX = 0;
        TreeGroup.TranslationY = 0;

        CrownLeft.BackgroundColor = normalLeafColor;
        CrownRight.BackgroundColor = normalLeafColor;
        CrownTop.BackgroundColor = normalTopLeafColor;

        Flower1.IsVisible = false;
        Flower2.IsVisible = false;
        Flower3.IsVisible = false;
        Flower4.IsVisible = false;
    }

    private uint GetDuration()
    {
        return (uint)Math.Clamp(SpeedStepper.Value, 500, 2000);
    }
}
