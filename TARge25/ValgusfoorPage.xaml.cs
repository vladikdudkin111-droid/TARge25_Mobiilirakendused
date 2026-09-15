namespace TARge25;

public partial class ValgusfoorPage : ContentPage
{
    private bool isTrafficLightOn = false;
    private bool isNightMode = false;
    private CancellationTokenSource? nightModeCancellation;

    public ValgusfoorPage()
    {
        InitializeComponent();
        SetAllLightsGray();
    }

    private async void OnTurnOnClicked(object? sender, EventArgs e)
    {
        isTrafficLightOn = true;

        if (isNightMode)
        {
            StartNightBlinking();
            StatusLabel.Text = "Öörežiim – kollane vilgub";
            return;
        }

        // Foor läheb sisse ja alustab punase tulega.
        SetActiveLight(RedLight, Colors.Red);
        StatusLabel.Text = "Seisa";
        await AnimateLightAsync(RedLight);
    }

    private void OnTurnOffClicked(object? sender, EventArgs e)
    {
        isTrafficLightOn = false;
        StopNightBlinking();
        SetAllLightsGray();
        StatusLabel.Text = "Lülita esmalt foor sisse";
    }

    private async void OnRedTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(RedLight, Colors.Red);
        StatusLabel.Text = "Seisa";
        await AnimateLightAsync(RedLight);
    }

    private async void OnYellowTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(YellowLight, Colors.Yellow);
        StatusLabel.Text = "Valmista";
        await AnimateLightAsync(YellowLight);
    }

    private async void OnGreenTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(GreenLight, Colors.LimeGreen);
        StatusLabel.Text = "Sõida";
        await AnimateLightAsync(GreenLight);
    }

    private void OnNightModeClicked(object? sender, EventArgs e)
    {
        if (isNightMode)
            DisableNightMode();
        else
            EnableNightMode();
    }

    private void EnableNightMode()
    {
        isNightMode = true;
        isTrafficLightOn = true;

        NightOverlay.Opacity = 0.62;
        StatusLabel.TextColor = Colors.White;
        StatusLabel.Text = "Öörežiim – kollane vilgub";
        TrafficLightBody.BackgroundColor = Color.FromArgb("#EE101010");
        NightModeButton.Text = "PÄEVAREŽIIM";

        SetAllLightsDarkGray();
        StartNightBlinking();
    }

    private void DisableNightMode()
    {
        isNightMode = false;
        isTrafficLightOn = false;

        StopNightBlinking();
        NightOverlay.Opacity = 0;
        StatusLabel.TextColor = Colors.Black;
        StatusLabel.Text = "Vali valgus.";
        TrafficLightBody.BackgroundColor = Color.FromArgb("#CC202020");
        NightModeButton.Text = "ÖÖREŽIIM";

        SetAllLightsGray();
    }

    private void StartNightBlinking()
    {
        StopNightBlinking();

        nightModeCancellation = new CancellationTokenSource();
        _ = BlinkYellowAsync(nightModeCancellation.Token);
    }

    private void StopNightBlinking()
    {
        if (nightModeCancellation is null)
            return;

        nightModeCancellation.Cancel();
        nightModeCancellation.Dispose();
        nightModeCancellation = null;
    }

    private async Task BlinkYellowAsync(CancellationToken token)
    {
        try
        {
            while (isNightMode && isTrafficLightOn && !token.IsCancellationRequested)
            {
                RedLight.BackgroundColor = Color.FromArgb("#303030");
                GreenLight.BackgroundColor = Color.FromArgb("#303030");
                YellowLight.BackgroundColor = Colors.Gold;

                await AnimateLightAsync(YellowLight);
                await Task.Delay(450, token);

                YellowLight.BackgroundColor = Color.FromArgb("#303030");
                await Task.Delay(450, token);
            }
        }
        catch (TaskCanceledException)
        {
            // Oodatud, kui öörežiim või foor välja lülitatakse.
        }
    }

    private void SetActiveLight(Frame activeLight, Color activeColor)
    {
        SetAllLightsGray();
        activeLight.BackgroundColor = activeColor;
    }

    private async Task AnimateLightAsync(Frame light)
    {
        // Väike animatsioon: aktiivne tuli suureneb ja tuhmub korraks.
#pragma warning disable CS0618
        await Task.WhenAll(
            light.ScaleTo(1.12, 140),
            light.FadeTo(0.65, 140)
        );

        await Task.WhenAll(
            light.ScaleTo(1.0, 140),
            light.FadeTo(1.0, 140)
        );
#pragma warning restore CS0618
    }

    private void SetAllLightsGray()
    {
        RedLight.BackgroundColor = Colors.Gray;
        YellowLight.BackgroundColor = Colors.Gray;
        GreenLight.BackgroundColor = Colors.Gray;
    }

    private void SetAllLightsDarkGray()
    {
        Color darkGray = Color.FromArgb("#303030");
        RedLight.BackgroundColor = darkGray;
        YellowLight.BackgroundColor = darkGray;
        GreenLight.BackgroundColor = darkGray;
    }

    protected override void OnDisappearing()
    {
        StopNightBlinking();
        base.OnDisappearing();
    }
}
