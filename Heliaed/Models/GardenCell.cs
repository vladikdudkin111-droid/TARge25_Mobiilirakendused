using Heliaed.Services;
using Heliaed.ViewModels;

namespace Heliaed.Models;

public sealed class GardenCell(int index) : ObservableObject
{
    private string kind = "empty";
    public int Index { get; } = index;
    public string Kind
    {
        get => kind;
        set
        {
            if (!Set(ref kind, value)) return;
            OnPropertyChanged(nameof(ImageFile));
            RefreshLanguage();
        }
    }
    public string ImageFile => Kind == "empty" ? "tile_blank.png" : $"tile_{Kind}.png";
    public string DisplayName => LocalizationService.Current[Kind];
    public string Description => $"{Index + 1}: {LocalizationService.Current[Kind == "empty" ? "EmptyCell" : Kind]}";
    public void RefreshLanguage()
    {
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(DisplayName));
    }
}
