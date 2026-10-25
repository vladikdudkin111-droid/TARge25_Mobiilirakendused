using Heliaed.Services;
using Heliaed.ViewModels;

namespace Heliaed.Views;

public sealed class MainTabs : TabbedPage
{
    private readonly MainViewModel viewModel;
    public MainTabs(MainViewModel viewModel)
    {
        this.viewModel = viewModel;
        BindingContext = viewModel;
        var garden = new GardenPage();
        Children.Add(garden);
        Children.Add(new LibraryPage());
        Children.Add(new SettingsPage());
        viewModel.EditorRequested += (_, _) => CurrentPage = garden;
        // Vaate navigeerimine jääb vaate vastutuseks.
        this.SetAppThemeColor(BarBackgroundColorProperty, Color.FromArgb("#E2EADD"), Color.FromArgb("#20372D"));
        this.SetAppThemeColor(BarTextColorProperty, Color.FromArgb("#193D33"), Color.FromArgb("#ECF3E8"));
        this.SetAppThemeColor(SelectedTabColorProperty, Color.FromArgb("#193D33"), Color.FromArgb("#C0F2BD"));
        this.SetAppThemeColor(UnselectedTabColorProperty, Color.FromArgb("#52645B"), Color.FromArgb("#ADC2B5"));
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitializeCommand.ExecuteAsync();
    }
}
