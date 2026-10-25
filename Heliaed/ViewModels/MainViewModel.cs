using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Heliaed.Models;
using Heliaed.Services;

namespace Heliaed.ViewModels;

// Vaademudel tunneb ainult andmeid, teenuseid ja käske, mitte UI-elemente.
public sealed class MainViewModel : ObservableObject
{
    private readonly ISceneRepository repository;
    private readonly IAudioService audio;
    private readonly LocalizationService texts;
    private GardenLayout layout = new();
    private string title = "", messageKey = "Welcome";
    private int editingId;
    private GardenScene? pendingDelete;
    private bool busy, initialized, active = true, dirty;
    public AppSettings Settings { get; }
    public ObservableCollection<GardenCell> Cells { get; } = new(Enumerable.Range(0, GardenLayout.CellCount).Select(i => new GardenCell(i)));
    public IReadOnlyList<GardenCell> Palette { get; } = GardenLayout.PictureKinds
        .Select((kind, index) => new GardenCell(index) { Kind = kind }).ToArray();
    public ObservableCollection<GardenScene> Scenes { get; } = new();
    public AsyncCommand InitializeCommand { get; }
    public AsyncCommand SaveCommand { get; }
    public AsyncCommand NewCommand { get; }
    public AsyncCommand ClearCommand { get; }
    public AsyncCommand PlaceCommand { get; }
    public AsyncCommand SelectKindCommand { get; }
    public AsyncCommand EditCommand { get; }
    public AsyncCommand RequestDeleteCommand { get; }
    public AsyncCommand ConfirmDeleteCommand { get; }
    public AsyncCommand CancelDeleteCommand { get; }
    public AsyncCommand PlayCommand { get; }
    public AsyncCommand PauseCommand { get; }
    public AsyncCommand LanguageCommand { get; }
    public event EventHandler? EditorRequested;
    public string Title
    {
        get => title;
        set { if (Set(ref title, value)) { dirty = true; RefreshEditor(); } }
    }
    public bool Busy { get => busy; private set { Set(ref busy, value); OnPropertyChanged(nameof(CanEdit)); RefreshCommands(); } }
    public bool CanEdit => !Busy;
    public bool HasPendingDelete => pendingDelete is not null;
    public string PendingDeleteTitle => pendingDelete?.Title ?? "";
    public string Status => texts[messageKey];
    public string Mode => texts[editingId == 0 ? "NewScene" : "EditingScene"] + (dirty ? " •" : "");
    public string SelectedText => texts["Selected"] + ": " + texts[Settings.SelectedKind];
    public string PlaybackText => texts[!Settings.SoundEnabled ? "Muted" : Settings.MusicPlaying ? "Playing" : "Paused"];
    public bool HasScenes => Scenes.Count > 0;
    public bool IsLibraryEmpty => !HasScenes;

    public MainViewModel(ISceneRepository repository, SettingsService settings, IAudioService audio)
    {
        this.repository = repository;
        this.audio = audio;
        Settings = settings.Values;
        texts = LocalizationService.Current;
        texts.SetLanguage(Settings.Language);
        InitializeCommand = Command(_ => InitializeAsync());
        SaveCommand = Command(_ => SaveAsync(), () => !Busy);
        NewCommand = Command(_ => { Reset(); return Task.CompletedTask; }, () => !Busy);
        ClearCommand = Command(_ => { layout = new(); UpdateCells(); dirty = true; RefreshEditor(); return Task.CompletedTask; }, () => !Busy);
        SelectKindCommand = Command(p => { Settings.SelectedKind = p as string ?? "leaf"; return Task.CompletedTask; });
        PlaceCommand = Command(async p =>
        {
            if (p is not GardenCell cell) return;
            layout.Place(cell.Index, Settings.SelectedKind);
            cell.Kind = Settings.SelectedKind;
            dirty = true;
            RefreshEditor();
            await audio.PlayPlacementAsync(Settings);
        }, () => !Busy);
        EditCommand = Command(p =>
        {
            if (p is GardenScene scene)
            {
                // Vigane kirje ei tohi olemasolevat redaktori sisu rikkuda.
                var loaded = GardenLayout.FromJson(scene.LayoutJson);
                layout = loaded; editingId = scene.Id; Title = scene.Title; dirty = false;
                UpdateCells(); RefreshEditor(); SetMessage("Loaded");
                EditorRequested?.Invoke(this, EventArgs.Empty);
            }
            return Task.CompletedTask;
        }, () => !Busy);
        RequestDeleteCommand = Command(p => { pendingDelete = p as GardenScene; RefreshDelete(); return Task.CompletedTask; }, () => !Busy);
        ConfirmDeleteCommand = Command(_ => DeleteAsync(), () => !Busy && HasPendingDelete);
        CancelDeleteCommand = Command(_ => { pendingDelete = null; RefreshDelete(); return Task.CompletedTask; }, () => !Busy);
        PlayCommand = Command(_ => { Settings.SoundEnabled = true; Settings.MusicPlaying = true; return ApplyAudioAsync(); });
        PauseCommand = Command(_ => { Settings.MusicPlaying = false; return ApplyAudioAsync(); });
        LanguageCommand = Command(p => { Settings.Language = p as string ?? "et"; return Task.CompletedTask; });
        Settings.PropertyChanged += SettingsChanged;
    }
    private AsyncCommand Command(Func<object?, Task> action, Func<bool>? enabled = null) => new(action, ReportError, enabled);
    private void SettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppSettings.Language))
        {
            texts.SetLanguage(Settings.Language);
            foreach (var cell in Cells) cell.RefreshLanguage();
            foreach (var shape in Palette) shape.RefreshLanguage();
            OnPropertyChanged(nameof(Status)); OnPropertyChanged(nameof(Mode));
        }
        OnPropertyChanged(nameof(SelectedText)); OnPropertyChanged(nameof(PlaybackText));
        if (e.PropertyName is nameof(AppSettings.SoundEnabled) or nameof(AppSettings.MusicPlaying) or nameof(AppSettings.Volume))
            _ = ApplyAudioSafelyAsync();
    }
    private async Task ApplyAudioSafelyAsync()
    {
        try { await ApplyAudioAsync(); }
        catch (Exception error) { ReportError(error); }
    }
    private Task ApplyAudioAsync() => audio.ApplyAsync(Settings, active);
    public async Task SetActiveAsync(bool value)
    {
        active = value;
        await ApplyAudioSafelyAsync();
    }
    public async Task InitializeAsync()
    {
        if (initialized || Busy) return;
        Busy = true;
        try { await ReloadAsync(); await ApplyAudioAsync(); initialized = true; }
        finally { Busy = false; }
    }
    private async Task ReloadAsync()
    {
        var records = await repository.GetAllAsync();
        Scenes.Clear(); foreach (var scene in records) Scenes.Add(scene);
        OnPropertyChanged(nameof(HasScenes)); OnPropertyChanged(nameof(IsLibraryEmpty));
    }
    private async Task SaveAsync()
    {
        if (Title.Trim().Length is < 1 or > 60) { SetMessage("NameRequired"); return; }
        Busy = true;
        try
        {
            var scene = new GardenScene { Id = editingId, Title = Title, LayoutJson = layout.ToJson() };
            await repository.SaveAsync(scene);
            editingId = scene.Id; Title = scene.Title; dirty = false;
            RefreshEditor(); await ReloadAsync(); SetMessage("Saved");
        }
        finally { Busy = false; }
    }
    private async Task DeleteAsync()
    {
        if (pendingDelete is not { } scene) return;
        Busy = true;
        try
        {
            await repository.DeleteAsync(scene.Id);
            if (editingId == scene.Id) Reset();
            pendingDelete = null; RefreshDelete();
            await ReloadAsync(); SetMessage("Deleted");
        }
        finally { Busy = false; }
    }
    private void Reset()
    {
        layout = new(); editingId = 0; Title = ""; dirty = false;
        UpdateCells(); RefreshEditor(); SetMessage("Welcome");
    }
    private void UpdateCells() { for (int i = 0; i < Cells.Count; i++) Cells[i].Kind = layout.Cells[i]; }
    private void RefreshEditor() => OnPropertyChanged(nameof(Mode));
    private void RefreshDelete()
    {
        OnPropertyChanged(nameof(HasPendingDelete)); OnPropertyChanged(nameof(PendingDeleteTitle));
        ConfirmDeleteCommand.Refresh();
    }
    private void RefreshCommands()
    {
        SaveCommand?.Refresh(); NewCommand?.Refresh(); ClearCommand?.Refresh(); PlaceCommand?.Refresh();
        EditCommand?.Refresh(); RequestDeleteCommand?.Refresh(); ConfirmDeleteCommand?.Refresh(); CancelDeleteCommand?.Refresh();
    }
    private void SetMessage(string key) { messageKey = key; OnPropertyChanged(nameof(Status)); }
    private void ReportError(Exception error)
    {
        Debug.WriteLine(error);
        SetMessage(error is FormatException or System.Text.Json.JsonException ? "InvalidLayout" : "OperationFailed");
    }
}
