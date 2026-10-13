using Microsoft.Maui.Controls.Shapes;

namespace TARge25.CarouselGallery;

// Leht kuvab programmeerimiskeeli interaktiivses CarouselView galeriis.
public class ProgrammingLanguagesPage : ContentPage
{
    // CarouselView näitab korraga ühte programmeerimiskeele kaarti.
    private readonly CarouselView carouselView;

    // IndicatorView näitab, mitu kaarti on ja milline kaart on aktiivne.
    private readonly IndicatorView indicatorView;

    // Keele valimiseks kasutame kahte stiilset nuppu.
    private readonly Button estonianButton;
    private readonly Button englishButton;

    // Lehe tekstid on väljad, et neid saaks keele vahetamisel muuta.
    private readonly Label titleLabel;
    private readonly Label subtitleLabel;
    private readonly Label autoScrollLabel;

    // Hetkel valitud keele kood.
    private string currentCultureCode = "et";

    // Automaatse kerimise peatamiseks kasutame CancellationTokenSource'i.
    private CancellationTokenSource? autoScrollCancellation;

    // Galeriis kuvatavad viis elementi.
    private List<ProgrammingLanguageItem> languages = new();

    public ProgrammingLanguagesPage()
    {
        Title = "CarouselView";
        BackgroundColor = Color.FromArgb("#08132D");

        // Sellel lehel kasutame oma ülariba, mitte tavalist heledat navigeerimisriba.
        NavigationPage.SetHasNavigationBar(this, false);

        // Vasakul olev ikoon loob kohe programmeerimisteemalise meeleolu.
        Label codeIconLabel = new Label
        {
            Text = "</>",
            FontSize = 34,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#89B4FF"),
            VerticalTextAlignment = TextAlignment.Center
        };

        // Lehe peamine pealkiri.
        titleLabel = new Label
        {
            FontSize = 23,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            LineBreakMode = LineBreakMode.WordWrap,
            MaxLines = 2
        };

        // Väiksem selgitav alapealkiri.
        subtitleLabel = new Label
        {
            FontSize = 13,
            TextColor = Color.FromArgb("#D7E2FF"),
            LineBreakMode = LineBreakMode.WordWrap,
            MaxLines = 2
        };

        // Eesti keele nupp.
        estonianButton = new Button
        {
            Text = "Eesti",
            CornerRadius = 22,
            Padding = new Thickness(18, 10),
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            MinimumWidthRequest = 110,
            HeightRequest = 42
        };
        estonianButton.Clicked += (_, _) => SetLanguage("et");

        // Inglise keele nupp.
        englishButton = new Button
        {
            Text = "English",
            CornerRadius = 22,
            Padding = new Thickness(18, 10),
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            MinimumWidthRequest = 110,
            HeightRequest = 42
        };
        englishButton.Clicked += (_, _) => SetLanguage("en");

        // IndicatorView kuvab CarouselView all väikesed lehekülje indikaatorid.
        indicatorView = new IndicatorView
        {
            HorizontalOptions = LayoutOptions.Center,
            IndicatorColor = Color.FromArgb("#BCC8EA"),
            SelectedIndicatorColor = Color.FromArgb("#A865FF"),
            IndicatorSize = 10,
            Margin = new Thickness(0, 4, 0, 0)
        };

        // Loome CarouselView ja seome selle IndicatorView-ga.
        carouselView = new CarouselView
        {
            HeightRequest = 458,
            HorizontalOptions = LayoutOptions.Fill,
            Loop = true,
            // Telefonis kuvame ühe tervikliku kaardi, mitte katkiseid naaberkaarte.
            PeekAreaInsets = new Thickness(0),
            IndicatorView = indicatorView
        };
        carouselView.ItemTemplate = CreateCardTemplate();

        // See Label teavitab kasutajat automaatsest 10-sekundilisest kerimisest.
        autoScrollLabel = new Label
        {
            FontSize = 13,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#D1DAF2")
        };

        // Pealkirja jaoks kasutame Gridi: tavaline HorizontalStackLayout ei piira
        // pika teksti laiust ja võib telefonis keelenupud ekraanilt välja lükata.
        Grid headingGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 12
        };
        headingGrid.Add(codeIconLabel, 0, 0);

        // Pealkiri ja selgitus on üksteise all ning mahuvad telefoni laiusesse.
        VerticalStackLayout headingText = new VerticalStackLayout
        {
            Spacing = 4,
            Children = { titleLabel, subtitleLabel }
        };
        headingGrid.Add(headingText, 1, 0);

        // Keele valik asub pealkirjast eraldi, et väikesel ekraanil midagi ei kattuks.
        HorizontalStackLayout languageButtonsLayout = new HorizontalStackLayout
        {
            Spacing = 8,
            HorizontalOptions = LayoutOptions.End,
            Children = { estonianButton, englishButton }
        };

        // Tagasinupp viib galeriist rakenduse tööde loendisse.
        Button backButton = new Button
        {
            Text = "←",
            FontSize = 24,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#44344680"),
            WidthRequest = 44,
            HeightRequest = 42,
            CornerRadius = 18,
            Padding = 0
        };
        backButton.Clicked += async (_, _) => await Navigation.PopAsync();

        // Esimeses reas on tagasinupp vasakul ja keelevalik paremal.
        Grid topBar = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 8
        };
        topBar.Add(backButton, 0, 0);
        topBar.Add(languageButtonsLayout, 1, 0);

        // Teises reas on ainult pealkiri ja selle selgitus.
        VerticalStackLayout headerLayout = new VerticalStackLayout
        {
            Spacing = 14,
            Children = { topBar, headingGrid }
        };

        // Paigutame tausta ja sisu kihiliselt üksteise peale.
        Grid pageLayout = new Grid();

        // Programmeerimisteemaline taustapilt täidab kogu ekraani.
        Image backgroundImage = new Image
        {
            Source = "programming_background.jpg",
            Aspect = Aspect.AspectFill,
            InputTransparent = true
        };
        pageLayout.Children.Add(backgroundImage);

        // Tume lilla-sinine gradient aitab teksti ja kaarte paremini esile tuua.
        BoxView backgroundOverlay = new BoxView
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                GradientStops =
                {
                    new GradientStop { Color = Color.FromArgb("#33040B23"), Offset = 0.0f },
                    new GradientStop { Color = Color.FromArgb("#9906132A"), Offset = 0.65f },
                    new GradientStop { Color = Color.FromArgb("#CC090E20"), Offset = 1.0f }
                }
            },
            InputTransparent = true
        };
        pageLayout.Children.Add(backgroundOverlay);

        // Koondame kõik nähtavad elemendid keritavasse paigutusse.
        ScrollView contentScroll = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16, 20, 16, 30),
                Spacing = 10,
                Children =
                {
                    headerLayout,
                    carouselView,
                    indicatorView,
                    autoScrollLabel
                }
            }
        };
        pageLayout.Children.Add(contentScroll);
        Content = pageLayout;

        // Käivitamisel valime vaikimisi eesti keele.
        SetLanguage("et");
    }

    // Loob ühe korduvkasutatava DataTemplate'i kõikidele kaartidele.
    private DataTemplate CreateCardTemplate()
    {
        return new DataTemplate(() =>
        {
            // Pilt kuvatakse kaardi ülaosas ja täidab suure osa ruumist.
            Image image = new Image
            {
                HeightRequest = 168,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 2)
            };
            image.SetBinding(Image.SourceProperty, nameof(ProgrammingLanguageItem.ImageSource));

            // Programmeerimiskeele nimi.
            Label nameLabel = new Label
            {
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#151B3B")
            };
            nameLabel.SetBinding(Label.TextProperty, nameof(ProgrammingLanguageItem.Name));

            // Programmeerimiskeele lühikirjeldus.
            Label descriptionLabel = new Label
            {
                FontSize = 15,
                TextColor = Color.FromArgb("#56607A"),
                LineBreakMode = LineBreakMode.WordWrap,
                MaxLines = 3
            };
            descriptionLabel.SetBinding(Label.TextProperty, nameof(ProgrammingLanguageItem.Description));

            // Kaardi alumine nupp kuvab detailsema info.
            Button detailButton = new Button
            {
                CornerRadius = 16,
                HeightRequest = 48,
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                Background = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#8A46FF"), 0.0f),
                        new GradientStop(Color.FromArgb("#5C66FF"), 1.0f)
                    },
                    new Point(0, 0),
                    new Point(1, 0))
            };

            // Nupu tõlge tuleb samast .resx failist nagu ülejäänud kaardi sisu.
            detailButton.SetBinding(Button.TextProperty, nameof(ProgrammingLanguageItem.ButtonText));

            // Kaardi sisu on jaotatud visuaalselt kaheks osaks: pilt ja tekstinuppude ala.
            Grid cardContent = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            // Ülemine ala saab kerge lilla-sinise tausta, et keskne logo paistaks paremini välja.
            Border topArea = new Border
            {
                Background = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb("#24104F"), 0.0f),
                        new GradientStop(Color.FromArgb("#5922A0"), 0.55f),
                        new GradientStop(Color.FromArgb("#1B1D67"), 1.0f)
                    },
                    new Point(0, 0),
                    new Point(1, 1)),
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(30, 30, 0, 0) },
                Padding = new Thickness(14, 16, 14, 10),
                Content = image
            };
            cardContent.Add(topArea, 0, 0);

            // Alumine valge sisuosa sisaldab pealkirja, kirjeldust ja nuppu.
            VerticalStackLayout bottomArea = new VerticalStackLayout
            {
                Padding = new Thickness(18, 15, 18, 20),
                Spacing = 10,
                Children =
                {
                    nameLabel,
                    descriptionLabel,
                    new Grid { HeightRequest = 4 },
                    detailButton
                }
            };
            cardContent.Add(bottomArea, 0, 1);

            // Border moodustab visuaalse kaardi neoonserva ja varjuga.
            Border card = new Border
            {
                Margin = new Thickness(10, 8, 10, 13),
                BackgroundColor = Colors.White,
                Stroke = Color.FromArgb("#A27BFF"),
                StrokeThickness = 2,
                StrokeShape = new RoundRectangle { CornerRadius = 30 },
                Shadow = new Shadow
                {
                    Brush = new SolidColorBrush(Color.FromArgb("#660A0F26")),
                    Radius = 26,
                    Offset = new Point(0, 12),
                    Opacity = 0.9f
                },
                Content = cardContent
            };

            // Nii nupp kui ka kogu kaart avavad detailse infoakna.
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
            tapGesture.Command = new Command<ProgrammingLanguageItem>(item =>
            {
                if (item != null)
                {
                    _ = ShowLanguageDetailsAsync(item);
                }
            });
            card.GestureRecognizers.Add(tapGesture);

            // Nupp kasutab sama andmeobjekti, mis terve kaart.
            detailButton.SetBinding(Button.CommandParameterProperty, ".");
            detailButton.Clicked += OnDetailButtonClicked;

            return card;
        });
    }

    // Käivitub siis, kui kasutaja vajutab kaardi alumist nuppu.
    private void OnDetailButtonClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ProgrammingLanguageItem item)
        {
            _ = ShowLanguageDetailsAsync(item);
        }
    }

    // Muudab aktiivset keelt ja värskendab kohe kogu lehe teksti.
    private void SetLanguage(string cultureCode)
    {
        currentCultureCode = cultureCode;
        GalleryResources.SetCulture(cultureCode);
        ApplyLanguage();
        UpdateLanguageButtonStyles();
    }

    // Uuendab kõik nähtavad tekstid vastavalt valitud keelele.
    private void ApplyLanguage()
    {
        Title = GalleryResources.Get("PageTitle");
        titleLabel.Text = GalleryResources.Get("GalleryTitle");
        subtitleLabel.Text = GalleryResources.Get("GallerySubtitle");
        autoScrollLabel.Text = GalleryResources.Get("AutoScrollInfo");

        // Loome viis kaarti uuesti, et kirjeldused oleksid õiges keeles.
        languages = CreateLanguageItems();
        carouselView.ItemsSource = languages;
        carouselView.Position = 0;
    }

    // Uuendab keele nuppude välimust vastavalt aktiivsele keelele.
    private void UpdateLanguageButtonStyles()
    {
        StyleLanguageButton(estonianButton, currentCultureCode == "et");
        StyleLanguageButton(englishButton, currentCultureCode == "en");
    }

    // Määrab valitud nupule erksama välimuse ja teisele tagasihoidlikuma stiili.
    private static void StyleLanguageButton(Button button, bool isActive)
    {
        if (isActive)
        {
            button.TextColor = Colors.White;
            button.Background = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#3F95FF"), 0.0f),
                    new GradientStop(Color.FromArgb("#245CFF"), 1.0f)
                },
                new Point(0, 0),
                new Point(1, 0));
        }
        else
        {
            button.TextColor = Colors.White;
            button.Background = new SolidColorBrush(Color.FromArgb("#30000000"));
        }
    }

    // Loob vähemalt viis ülesandes nõutud elementi.
    private static List<ProgrammingLanguageItem> CreateLanguageItems()
    {
        return new List<ProgrammingLanguageItem>
        {
            new()
            {
                ImageSource = "csharp_gallery.svg",
                Name = "C#",
                ButtonText = GalleryResources.Get("DetailsButton"),
                Description = GalleryResources.Get("CSharpDescription"),
                Detail = GalleryResources.Get("CSharpDetail")
            },
            new()
            {
                ImageSource = "python_gallery.svg",
                Name = "Python",
                ButtonText = GalleryResources.Get("DetailsButton"),
                Description = GalleryResources.Get("PythonDescription"),
                Detail = GalleryResources.Get("PythonDetail")
            },
            new()
            {
                ImageSource = "javascript_gallery.svg",
                Name = "JavaScript",
                ButtonText = GalleryResources.Get("DetailsButton"),
                Description = GalleryResources.Get("JavaScriptDescription"),
                Detail = GalleryResources.Get("JavaScriptDetail")
            },
            new()
            {
                ImageSource = "java_gallery.svg",
                Name = "Java",
                ButtonText = GalleryResources.Get("DetailsButton"),
                Description = GalleryResources.Get("JavaDescription"),
                Detail = GalleryResources.Get("JavaDetail")
            },
            new()
            {
                ImageSource = "cpp_gallery.svg",
                Name = "C++",
                ButtonText = GalleryResources.Get("DetailsButton"),
                Description = GalleryResources.Get("CppDescription"),
                Detail = GalleryResources.Get("CppDetail")
            }
        };
    }

    // Näitab vajutatud programmeerimiskeele detailset infot DisplayAlert aknas.
    private async Task ShowLanguageDetailsAsync(ProgrammingLanguageItem item)
    {
        await DisplayAlertAsync(
            item.Name,
            item.Detail,
            GalleryResources.Get("CloseButton"));
    }

    // Lehele tulles käivitame automaatse kerimise.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartAutoScroll();
    }

    // Lehelt lahkudes peatame taustal töötava automaatse kerimise.
    protected override void OnDisappearing()
    {
        StopAutoScroll();
        base.OnDisappearing();
    }

    // Käivitab taimeri, mis liigub iga 10 sekundi järel järgmisele kaardile.
    private void StartAutoScroll()
    {
        StopAutoScroll();
        autoScrollCancellation = new CancellationTokenSource();
        _ = AutoScrollAsync(autoScrollCancellation.Token);
    }

    // Peatab automaatse kerimise, kui kasutaja lehelt lahkub.
    private void StopAutoScroll()
    {
        if (autoScrollCancellation == null)
        {
            return;
        }

        autoScrollCancellation.Cancel();
        autoScrollCancellation.Dispose();
        autoScrollCancellation = null;
    }

    // Vahetab aktiivset CarouselView kaarti iga 10 sekundi järel.
    private async Task AutoScrollAsync(CancellationToken token)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));

        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                if (languages.Count == 0)
                {
                    continue;
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    int nextPosition = (carouselView.Position + 1) % languages.Count;
                    carouselView.Position = nextPosition;
                });
            }
        }
        catch (OperationCanceledException)
        {
            // See erind on normaalne, kui kasutaja lahkub lehelt ja taimer peatatakse.
        }
    }
}
