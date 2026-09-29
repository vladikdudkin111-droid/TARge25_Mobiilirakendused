namespace TARge25;

// Eraldi reeglite leht teeb rakenduse mitmeleheküljeliseks,
// nagu ülesande nõuetes on kirjas.
public class TicTacToeRulesPage : ContentPage
{
    public TicTacToeRulesPage()
    {
        // NavigationPage ülemise riba pealkiri.
        Title = "Reeglid";
        BackgroundColor = Colors.White;

        // Lehe suur pealkiri.
        Label title = new Label
        {
            Text = "Trips-Traps-Trull – reeglid",
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        };

        // Reeglite tekst selgitab kasutajale, kuidas mängida.
        Label rules = new Label
        {
            Text =
                "• Mängivad kaks mängijat: X ja O.\n\n" +
                "• Mängijad teevad käike kordamööda.\n\n" +
                "• Puuduta tühja ruutu, et asetada sinna oma sümbol.\n\n" +
                "• Võidab mängija, kes saab kolm oma sümbolit järjest – horisontaalselt, vertikaalselt või diagonaalis.\n\n" +
                "• Kui kõik ruudud on täidetud ja võitjat pole, on tulemuseks viik.\n\n" +
                "• „Kes alustab?” valib juhuslikult esimese mängija.",
            FontSize = 18,
            TextColor = Colors.Black,
            LineBreakMode = LineBreakMode.WordWrap
        };

        // TAGASI nupp viib mängu lehele tagasi.
        Button backButton = new Button
        {
            Text = "TAGASI",
            BackgroundColor = Color.FromArgb("#D9D9D9"),
            TextColor = Colors.Black,
            CornerRadius = 8
        };

        // PopAsync eemaldab reeglite lehe navigeerimise ajaloost.
        backButton.Clicked += async (sender, e) => await Navigation.PopAsync();

        // ScrollView võimaldab reegleid kerida väiksel ekraanil.
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 24,
                Spacing = 22,
                Children = { title, rules, backButton }
            }
        };
    }
}
