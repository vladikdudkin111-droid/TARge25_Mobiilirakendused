using Microsoft.Maui.Layouts;

namespace TARge25;

public class LumememmPage : ContentPage
{
    private readonly AbsoluteLayout snowmanLayout;
    private readonly Frame bucket;
    private readonly Frame head;
    private readonly Frame bodyMiddle;
    private readonly Frame bodyBottom;
    private readonly Picker actionPicker;
    private readonly Label actionLabel;
    private readonly Label opacityLabel;
    private readonly Label speedLabel;
    private readonly Slider opacitySlider;
    private readonly Stepper speedStepper;
    private readonly Button actionButton;
    private readonly List<VisualElement> snowmanElements = new();
    private readonly Random random = new();

    public LumememmPage()
    {
        Title = "Lumememm";
        BackgroundColor = Color.FromArgb("#EAF6FF");

        Label titleLabel = new Label
        {
            Text = "⛄ Lumememm",
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Color.FromArgb("#17324D")
        };

        actionLabel = new Label
        {
            Text = "Valitud tegevus: -",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Colors.DarkSlateGray
        };

        actionPicker = new Picker
        {
            Title = "Vali tegevus",
            HorizontalOptions = LayoutOptions.Fill
        };

        actionPicker.Items.Add("Peida lumememm");
        actionPicker.Items.Add("Näita lumememm");
        actionPicker.Items.Add("Muuda värvi");
        actionPicker.Items.Add("Sulata");
        actionPicker.Items.Add("Tantsi");
        actionPicker.SelectedIndexChanged += ActionPicker_SelectedIndexChanged;

        actionButton = new Button
        {
            Text = "KÄIVITA TEGEVUS",
            HeightRequest = 52,
            CornerRadius = 10,
            HorizontalOptions = LayoutOptions.Fill
        };
        actionButton.Clicked += ActionButton_Clicked;

        opacityLabel = new Label
        {
            Text = "Heledus / läbipaistvus: 100%",
            FontSize = 15,
            TextColor = Colors.DarkSlateGray
        };

        opacitySlider = new Slider
        {
            Minimum = 0,
            Maximum = 1,
            Value = 1,
            HorizontalOptions = LayoutOptions.Fill
        };
        opacitySlider.ValueChanged += OpacitySlider_ValueChanged;

        speedLabel = new Label
        {
            Text = "Kiirus: 3",
            FontSize = 15,
            TextColor = Colors.DarkSlateGray
        };

        speedStepper = new Stepper
        {
            Minimum = 1,
            Maximum = 5,
            Increment = 1,
            Value = 3,
            HorizontalOptions = LayoutOptions.Start
        };
        speedStepper.ValueChanged += SpeedStepper_ValueChanged;

        // Lumememm tehakse AbsoluteLayout'i sisse, et kõiki osi saaks täpselt paigutada.
        snowmanLayout = new AbsoluteLayout
        {
            WidthRequest = 360,
            HeightRequest = 525,
            HorizontalOptions = LayoutOptions.Center
        };

        bodyBottom = CreateSnowball(220, 200);
        bodyMiddle = CreateSnowball(170, 170);
        head = CreateSnowball(130, 130);

        bucket = new Frame
        {
            WidthRequest = 110,
            HeightRequest = 55,
            Padding = 0,
            CornerRadius = 8,
            BackgroundColor = Colors.DarkSlateBlue,
            BorderColor = Colors.MidnightBlue,
            HasShadow = true
        };

        // SetLayoutBounds määrab X, Y, laiuse ja kõrguse.
        // SetLayoutFlags(None) tähendab, et kasutame siin täpseid piksliväärtusi.
        AddToSnowman(bodyBottom, new Rect(70, 300, 220, 200));
        AddToSnowman(bodyMiddle, new Rect(95, 175, 170, 170));
        AddToSnowman(head, new Rect(115, 65, 130, 130));
        AddToSnowman(bucket, new Rect(125, 18, 110, 55));

        // Silmad
        Frame leftEye = CreateSmallCircle(18, Colors.Black);
        Frame rightEye = CreateSmallCircle(18, Colors.Black);
        AddToSnowman(leftEye, new Rect(145, 103, 18, 18));
        AddToSnowman(rightEye, new Rect(197, 103, 18, 18));

        // Porgandinina
        BoxView nose = new BoxView
        {
            Color = Colors.Orange,
            Rotation = -10
        };
        AddToSnowman(nose, new Rect(174, 125, 38, 10));

        // Nööbid
        Frame button1 = CreateSmallCircle(18, Colors.Black);
        Frame button2 = CreateSmallCircle(18, Colors.Black);
        Frame button3 = CreateSmallCircle(18, Colors.Black);
        AddToSnowman(button1, new Rect(171, 225, 18, 18));
        AddToSnowman(button2, new Rect(171, 270, 18, 18));
        AddToSnowman(button3, new Rect(171, 350, 18, 18));

        // Ämbri serv
        BoxView bucketEdge = new BoxView
        {
            Color = Colors.MidnightBlue
        };
        AddToSnowman(bucketEdge, new Rect(112, 65, 136, 10));

        VerticalStackLayout controls = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = new Thickness(20, 20, 20, 5),
            Children =
            {
                titleLabel,
                actionLabel,
                actionPicker,
                actionButton,
                opacityLabel,
                opacitySlider,
                speedLabel,
                speedStepper
            }
        };

        VerticalStackLayout pageLayout = new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                controls,
                snowmanLayout
            }
        };

        Content = new ScrollView
        {
            Content = pageLayout
        };
    }

    private Frame CreateSnowball(double width, double height)
    {
        return new Frame
        {
            WidthRequest = width,
            HeightRequest = height,
            Padding = 0,
            CornerRadius = (float)(Math.Min(width, height) / 2),
            BackgroundColor = Colors.White,
            BorderColor = Colors.LightGray,
            HasShadow = true
        };
    }

    private static Frame CreateSmallCircle(double size, Color color)
    {
        return new Frame
        {
            WidthRequest = size,
            HeightRequest = size,
            Padding = 0,
            CornerRadius = (float)(size / 2),
            BackgroundColor = color,
            HasShadow = false
        };
    }

    private void AddToSnowman(VisualElement element, Rect bounds)
    {
        AbsoluteLayout.SetLayoutBounds(element, bounds);
        AbsoluteLayout.SetLayoutFlags(element, AbsoluteLayoutFlags.None);
        snowmanLayout.Children.Add(element);
        snowmanElements.Add(element);
    }

    private void ActionPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (actionPicker.SelectedItem is string selectedAction)
        {
            actionLabel.Text = "Valitud tegevus: " + selectedAction;
        }
    }

    private async void ActionButton_Clicked(object? sender, EventArgs e)
    {
        if (actionPicker.SelectedItem is not string selectedAction)
        {
            await DisplayAlertAsync("Teade", "Vali kõigepealt Pickerist tegevus.", "OK");
            return;
        }

        actionLabel.Text = "Tegevus: " + selectedAction;
        actionButton.IsEnabled = false;

        try
        {
            switch (selectedAction)
            {
                case "Peida lumememm":
                    HideSnowman();
                    break;

                case "Näita lumememm":
                    ShowSnowman();
                    break;

                case "Muuda värvi":
                    await ChangeSnowmanColorAsync();
                    break;

                case "Sulata":
                    await MeltSnowmanAsync();
                    break;

                case "Tantsi":
                    await DanceSnowmanAsync();
                    break;
            }
        }
        finally
        {
            actionButton.IsEnabled = true;
        }
    }

    private void HideSnowman()
    {
        snowmanLayout.IsVisible = false;
        actionLabel.Text = "Tegevus: lumememm on peidetud";
    }

    private void ShowSnowman()
    {
        snowmanLayout.IsVisible = true;
        snowmanLayout.Opacity = 1;
        snowmanLayout.Scale = 1;
        snowmanLayout.TranslationX = 0;
        actionLabel.Text = "Tegevus: lumememm on nähtav";
    }

    private async Task ChangeSnowmanColorAsync()
    {
        bool confirmed = await DisplayAlertAsync(
            "Muuda värvi",
            "Kas soovid lumememme värvi juhuslikult muuta?",
            "Jah",
            "Ei");

        if (!confirmed)
        {
            actionLabel.Text = "Värvi muutmine tühistati";
            return;
        }

        Color[] colors =
        {
            Colors.LightBlue,
            Colors.LightPink,
            Colors.LightGreen,
            Colors.LightYellow,
            Colors.Lavender,
            Colors.MistyRose,
            Colors.White
        };

        Color randomColor = colors[random.Next(colors.Length)];
        head.BackgroundColor = randomColor;
        bodyMiddle.BackgroundColor = randomColor;
        bodyBottom.BackgroundColor = randomColor;

        snowmanLayout.IsVisible = true;
        snowmanLayout.Opacity = 1;
        snowmanLayout.Scale = 1;
        actionLabel.Text = "Tegevus: lumememme värv muudeti";
    }

    private async Task MeltSnowmanAsync()
    {
        ShowSnowman();
        uint duration = GetAnimationDuration(4);
        actionLabel.Text = "Tegevus: lumememm sulab...";

        await Task.WhenAll(
            snowmanLayout.FadeTo(0, duration),
            snowmanLayout.ScaleTo(0.55, duration)
        );

        actionLabel.Text = "Tegevus: lumememm sulas ära";
    }

    private async Task DanceSnowmanAsync()
    {
        ShowSnowman();
        uint duration = GetAnimationDuration(1);
        actionLabel.Text = "Tegevus: lumememm tantsib";

        await snowmanLayout.TranslateTo(-55, 0, duration);
        await snowmanLayout.TranslateTo(55, 0, duration);
        await snowmanLayout.TranslateTo(-40, 0, duration);
        await snowmanLayout.TranslateTo(40, 0, duration);
        await snowmanLayout.TranslateTo(0, 0, duration);

        actionLabel.Text = "Tegevus: tants lõpetatud";
    }

    private uint GetAnimationDuration(double multiplier)
    {
        // Suurem Stepperi väärtus = kiirem animatsioon = vähem millisekundeid.
        double baseDuration = 700.0 / speedStepper.Value;
        return (uint)Math.Max(100, baseDuration * multiplier);
    }

    private void OpacitySlider_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        double opacity = e.NewValue;

        foreach (VisualElement element in snowmanElements)
        {
            element.Opacity = opacity;
        }

        opacityLabel.Text = $"Heledus / läbipaistvus: {(int)(opacity * 100)}%";
    }

    private void SpeedStepper_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        speedLabel.Text = $"Kiirus: {(int)e.NewValue}";
    }
}
