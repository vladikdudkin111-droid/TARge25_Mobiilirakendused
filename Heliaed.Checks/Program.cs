using System.ComponentModel;
using Heliaed.Models;
using Heliaed.Services;
using Heliaed.ViewModels;

var assertions = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
    assertions++;
}

var folder = Path.Combine(Path.GetTempPath(), "Heliaed-Checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(folder);
try
{
    var path = Path.Combine(folder, "test.db3");
    var layout = new GardenLayout();
    Check(layout.Cells.Count == 36 && GardenLayout.Size == 6, "Board is 6 by 6");
    Check(GardenLayout.PictureKinds.Count == 20 && GardenLayout.PictureKinds.Distinct().Count() == 20, "Twenty distinct pictures");
    Check(GardenLayout.PictureKinds.Contains("house") && !GardenLayout.PictureKinds.Contains("fish"), "House replaces fish in palette");
    foreach (var oldLength in new[] { 9, 36, 100 })
    {
        var oldValues = Enumerable.Repeat("empty", oldLength).ToArray();
        oldValues[0] = oldValues[^1] = "fish";
        var converted = GardenLayout.FromJson(System.Text.Json.JsonSerializer.Serialize(oldValues));
        var convertedValues = System.Text.Json.JsonSerializer.Deserialize<string[]>(converted.ToJson())!;
        Check(converted.Cells[0] == "house" && convertedValues.Count(x => x == "house") == 2 && !convertedValues.Contains("fish"), "Fish migration in saved layout: " + oldLength);
    }
    Check(!GardenLayout.PictureKinds.Contains("empty") && GardenLayout.Kinds.Contains("empty"), "Eraser is an additional tool");
    var pictureProbe = new GardenLayout();
    for (int i = 0; i < GardenLayout.PictureKinds.Count; i++)
    {
        var kind = GardenLayout.PictureKinds[i];
        pictureProbe.Place(i, kind);
        Check(GardenLayout.FromJson(pictureProbe.ToJson()).Cells[i] == kind, "Picture round trip: " + kind);
    }
    layout.Place(35, "star");
    Check(GardenLayout.FromJson(layout.ToJson()).Cells[35] == "star", "Last cell is editable and persisted");
    foreach (var invalidIndex in new[] { -1, 36 })
    {
        var refused = false;
        try { layout.Place(invalidIndex, "leaf"); } catch (ArgumentOutOfRangeException) { refused = true; }
        Check(refused, "Out-of-board placement rejected");
    }
    var legacyJson = "[\"leaf\",\"empty\",\"moon\",\"flower\",\"leaf\",\"empty\",\"moon\",\"empty\",\"flower\"]";
    var legacy = GardenLayout.FromJson(legacyJson);
    Check(legacy.Cells.Count == 36 && legacy.Cells[2] == "moon" && legacy.Cells[6] == "flower" && legacy.Cells[7] == "leaf" && legacy.Cells[14] == "flower", "Old 3 by 3 rows map into top-left corner");
    Check(Enumerable.Range(0, 36).Where(i => i / 6 >= 3 || i % 6 >= 3).All(i => legacy.Cells[i] == "empty"), "New area of migrated garden is empty");
    Check(GardenLayout.FromJson(legacy.ToJson()).Cells.SequenceEqual(legacy.Cells), "Migrated layout round trip");
    var largeValues = Enumerable.Repeat("empty", 100).ToArray();
    largeValues[55] = "sun";
    largeValues[99] = "star";
    var oldLarge = GardenLayout.FromJson(System.Text.Json.JsonSerializer.Serialize(largeValues));
    Check(oldLarge.Cells.Count == 36 && oldLarge.Cells[35] == "sun", "10 by 10 crops to top-left 6 by 6 with correct rows");
    oldLarge.Place(35, "butterfly");
    var preservedLarge = System.Text.Json.JsonSerializer.Deserialize<string[]>(oldLarge.ToJson())!;
    Check(preservedLarge[55] == "butterfly" && preservedLarge[99] == "star", "Editing old board preserves hidden outer cells");
    Check(GardenLayout.FromJson(oldLarge.ToJson()).Cells[35] == "butterfly", "Old large board round trip");
    Check(System.Text.Json.JsonSerializer.Deserialize<string[]>(new GardenLayout().ToJson())!.Length == 36, "New gardens save 36 cells");
    var migrationPath = Path.Combine(folder, "migration.db3");
    await using (var migrationDb = new SceneRepository(migrationPath))
    {
        var oldScene = new GardenScene { Title = "Vana aed", LayoutJson = legacyJson };
        await migrationDb.SaveAsync(oldScene);
        var read = (await migrationDb.GetAllAsync()).Single();
        var migrated = GardenLayout.FromJson(read.LayoutJson);
        migrated.Place(35, "butterfly");
        read.LayoutJson = migrated.ToJson();
        await migrationDb.SaveAsync(read);
        var after = (await migrationDb.GetAllAsync()).Single();
        Check(after.Id == oldScene.Id && GardenLayout.FromJson(after.LayoutJson).Cells[35] == "butterfly", "Old SQLite entry expands without losing ID");
    }
    layout.Place(0, "moon"); layout.Place(8, "flower");
    Check(GardenLayout.FromJson(layout.ToJson()).Cells[8] == "flower", "Layout round trip");
    foreach (var invalid in new[] { "[]", "null", "[\"invalid\"]", "broken" })
    {
        bool rejected = false;
        try { GardenLayout.FromJson(invalid); }
        catch (Exception e) when (e is FormatException or System.Text.Json.JsonException) { rejected = true; }
        Check(rejected, "Invalid layout rejected");
    }
    await using (var db = new SceneRepository(path))
    {
        Check((await db.GetAllAsync()).Count == 0, "Empty database");
        var scene = new GardenScene { Title = "  Esimene aed  ", LayoutJson = layout.ToJson() };
        await db.SaveAsync(scene);
        Check(scene.Id > 0 && scene.Title == "Esimene aed", "Insert and trim");
        scene.Title = "Muudetud aed"; layout.Place(0, "leaf"); scene.LayoutJson = layout.ToJson();
        await db.SaveAsync(scene);
        Check((await db.GetAllAsync()).Count == 1, "Update preserves ID");
        Check((await db.GetAllAsync())[0].Title == "Muudetud aed", "Updated title");
        var bad = new GardenScene { Title = " ", LayoutJson = layout.ToJson() };
        var refused = false;
        try { await db.SaveAsync(bad); } catch (ArgumentException) { refused = true; }
        Check(refused && (await db.GetAllAsync()).Count == 1, "Invalid title never inserted");
        await Task.WhenAll(Enumerable.Range(1, 8).Select(i => db.SaveAsync(new GardenScene { Title = $"Aed {i}", LayoutJson = layout.ToJson() })));
        Check((await db.GetAllAsync()).Count == 9, "Concurrent CRUD is serialized");
    }
    await using (var reopened = new SceneRepository(path))
    {
        var all = await reopened.GetAllAsync();
        Check(all.Count == 9, "Database survives reopening");
        Check(all.Any(x => x.Title == "Muudetud aed" && GardenLayout.FromJson(x.LayoutJson).Cells[0] == "leaf"), "Layout and title persist");
        Check(all.All(x => GardenLayout.FromJson(x.LayoutJson).Cells[35] == "star"), "Last row survives reopening SQLite");
        foreach (var scene in all) await reopened.DeleteAsync(scene.Id);
        Check((await reopened.GetAllAsync()).Count == 0, "Delete persists");
    }

    var store = new MemoryStore();
    var settings = new SettingsService(store);
    settings.Values.Language = "en"; settings.Values.IsDark = true;
    settings.Values.SoundEnabled = false; settings.Values.MusicPlaying = true;
    settings.Values.Volume = .7; settings.Values.SelectedKind = "moon";
    var restored = new SettingsService(store).Values;
    Check(restored.Language == "en" && restored.IsDark && !restored.SoundEnabled && restored.MusicPlaying, "Settings restored");
    Check(restored.Volume == .7 && restored.SelectedKind == "moon", "Volume and tool restored");
    restored.Volume = 99; Check(restored.Volume == 1, "Volume clamped");
    restored.Volume = double.NaN; Check(restored.Volume == .35, "NaN volume rejected");
    restored.SelectedKind = "invalid"; Check(restored.SelectedKind == "leaf", "Invalid tool reset");
    restored.SelectedKind = "fish"; Check(restored.SelectedKind == "house", "Old fish tool restores as house");

    var loc = LocalizationService.Current;
    var notifications = 0;
    loc.PropertyChanged += (_, _) => notifications++;
    loc.SetLanguage("et"); Check(loc["Save"] == "Salvesta", "ET resx loaded");
    loc.SetLanguage("en"); Check(loc["Save"] == "Save" && notifications == 2, "EN resx and live notification");
    foreach (var language in new[] { "et", "en" })
    {
        loc.SetLanguage(language);
        foreach (var kind in GardenLayout.PictureKinds)
            Check(loc[kind] != kind, "Picture translated: " + language + "/" + kind);
    }
    foreach (var key in new[] { "Garden", "SettingsTitle", "OperationFailed", "InvalidLayout", "Selected", "leaf", "RestoreHint" })
        Check(!string.IsNullOrWhiteSpace(loc[key]), "Resource key exists: " + key);

    var fakeAudio = new FakeAudio();
    await using (var repo = new SceneRepository(path))
    {
        var vm = new MainViewModel(repo, settings, fakeAudio);
        await vm.InitializeCommand.ExecuteAsync();
        Check(vm.Cells.Count == 36 && vm.Palette.Count == 20, "ViewModel exposes whole board and palette");
        Check(vm.IsLibraryEmpty, "VM starts empty");
        await vm.SaveCommand.ExecuteAsync();
        Check(vm.Status == "Enter a name with 1–60 characters." && vm.Scenes.Count == 0, "VM name validation");
        vm.Title = "My garden";
        await vm.SelectKindCommand.ExecuteAsync("flower");
        await vm.PlaceCommand.ExecuteAsync(vm.Cells[4]);
        Check(vm.Cells[4].Kind == "flower" && fakeAudio.Placements == 1, "Tap-to-place command");
        await vm.SaveCommand.ExecuteAsync();
        Check(vm.Scenes.Count == 1 && GardenLayout.FromJson(vm.Scenes[0].LayoutJson).Cells[4] == "flower", "VM saves layout");
        await vm.SelectKindCommand.ExecuteAsync("star");
        await vm.PlaceCommand.ExecuteAsync(vm.Cells[35]);
        await vm.SaveCommand.ExecuteAsync();
        Check(GardenLayout.FromJson(vm.Scenes[0].LayoutJson).Cells[35] == "star", "ViewModel saves bottom-right cell");
        var id = vm.Scenes[0].Id;
        await vm.EditCommand.ExecuteAsync(vm.Scenes[0]);
        vm.Title = "Updated garden";
        await vm.SelectKindCommand.ExecuteAsync("empty");
        await vm.PlaceCommand.ExecuteAsync(vm.Cells[4]);
        await vm.SaveCommand.ExecuteAsync();
        Check(vm.Scenes.Count == 1 && vm.Scenes[0].Id == id && vm.Scenes[0].Title == "Updated garden", "VM edits same record");
        Check(GardenLayout.FromJson(vm.Scenes[0].LayoutJson).Cells[4] == "empty", "Eraser persisted");
        await vm.RequestDeleteCommand.ExecuteAsync(vm.Scenes[0]);
        Check(vm.HasPendingDelete && (await repo.GetAllAsync()).Count == 1, "Delete requires confirmation");
        await vm.CancelDeleteCommand.ExecuteAsync();
        Check(!vm.HasPendingDelete && vm.Scenes.Count == 1, "Cancel deletion");
        await vm.RequestDeleteCommand.ExecuteAsync(vm.Scenes[0]);
        await vm.ConfirmDeleteCommand.ExecuteAsync();
        Check(vm.Scenes.Count == 0 && vm.IsLibraryEmpty, "Confirmed delete updates collection");
        await vm.LanguageCommand.ExecuteAsync("et");
        Check(loc.Language == "et" && vm.Status == "Aed on kustutatud.", "Language refreshes current status");
        await vm.PlayCommand.ExecuteAsync();
        Check(settings.Values.MusicPlaying && settings.Values.SoundEnabled, "Play restores sound");
        await vm.SetActiveAsync(false);
        Check(!fakeAudio.Active && settings.Values.MusicPlaying, "Suspend keeps playback preference");
        await vm.SetActiveAsync(true);
        Check(fakeAudio.Active, "Resume applies audio preference");
        await vm.PauseCommand.ExecuteAsync();
        Check(!settings.Values.MusicPlaying, "Pause persisted");
    }
    var errors = 0;
    var command = new AsyncCommand(async _ => { await Task.Yield(); throw new IOException("Katse"); }, _ => errors++);
    await command.ExecuteAsync();
    Check(errors == 1 && command.CanExecute(null), "Async errors caught and command reenabled");
    Console.WriteLine($"PASS: {assertions} assertions. SQLite CRUD/reopen, settings, ET/EN, MVVM, placement, audio intent, errors.");
}
finally
{
    SQLite.SQLiteAsyncConnection.ResetPool();
    Directory.Delete(folder, true);
}

sealed class MemoryStore : ISettingsStore
{
    private readonly Dictionary<string, string> data = new();
    public string Get(string key, string fallback) => data.GetValueOrDefault(key, fallback);
    public void Set(string key, string value) => data[key] = value;
}
sealed class FakeAudio : IAudioService
{
    public bool Active { get; private set; }
    public int Placements { get; private set; }
    public Task ApplyAsync(AppSettings settings, bool active) { Active = active; return Task.CompletedTask; }
    public Task PlayPlacementAsync(AppSettings settings) { Placements++; return Task.CompletedTask; }
}
