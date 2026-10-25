using Heliaed.Services;
using Heliaed.ViewModels;
using Heliaed.Views;

namespace Heliaed;

public partial class App : Application
{
    private readonly IServiceProvider services;
    private readonly ThemeService theme;
    private readonly MainViewModel viewModel;
    private readonly AudioService audio;
    private readonly SceneRepository repository;
    public App(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        theme = services.GetRequiredService<ThemeService>();
        viewModel = services.GetRequiredService<MainViewModel>();
        audio = services.GetRequiredService<AudioService>();
        repository = services.GetRequiredService<SceneRepository>();
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        theme.Apply();
        var window = new Window(services.GetRequiredService<MainTabs>()) { Title = "Heliaed", Width = 720, Height = 940, MinimumWidth = 360, MinimumHeight = 600 };
        // Taustale minek peatab heli, kuid ei muuda kasutaja esitusvalikut.
        window.Stopped += async (_, _) => await viewModel.SetActiveAsync(false);
        window.Resumed += async (_, _) => await viewModel.SetActiveAsync(true);
        window.Destroying += async (_, _) =>
        {
            await viewModel.SetActiveAsync(false);
            audio.Dispose();
            await repository.DisposeAsync();
        };
        return window;
    }
}
