namespace TARge25;

public class MenuPage : ContentPage
{
    public MenuPage()
    {
        Title = "Tööd";
        BackgroundColor = Colors.White;

        Label titleLabel = new Label
        {
            Text = "Vali töö",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.Black,
            Margin = new Thickness(0, 20, 0, 15)
        };

        Button treeButton = CreateMenuButton("Puu hooajad ja elutsükkel");
        Button snowmanButton = CreateMenuButton("Lumememm");
        Button popupButton = CreateMenuButton("Pop-up aknad");
        Button trafficLightButton = CreateMenuButton("Valgusfoor");

        treeButton.Clicked += async (sender, e) =>
            await Navigation.PushAsync(new TreePage());

        snowmanButton.Clicked += async (sender, e) =>
            await Navigation.PushAsync(new LumememmPage());

        popupButton.Clicked += async (sender, e) =>
            await Navigation.PushAsync(new PopUpPage());

        trafficLightButton.Clicked += async (sender, e) =>
            await Navigation.PushAsync(new ValgusfoorPage());

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(25, 10),
                Spacing = 14,
                Children =
                {
                    titleLabel,
                    treeButton,
                    snowmanButton,
                    popupButton,
                    trafficLightButton
                }
            }
        };
    }

    private static Button CreateMenuButton(string text)
    {
        return new Button
        {
            Text = text,
            FontSize = 18,
            HeightRequest = 58,
            BackgroundColor = Color.FromArgb("#D9D9D9"),
            TextColor = Colors.Black,
            CornerRadius = 8,
            HorizontalOptions = LayoutOptions.Fill
        };
    }
}
