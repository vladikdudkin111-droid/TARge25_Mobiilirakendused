using Heliaed.Models;

namespace Heliaed.Services;

public interface ISettingsStore
{
    string Get(string key, string fallback);
    void Set(string key, string value);
}
public interface ISceneRepository
{
    Task<List<GardenScene>> GetAllAsync();
    Task SaveAsync(GardenScene scene);
    Task DeleteAsync(int id);
}
public interface IAudioService
{
    Task ApplyAsync(AppSettings settings, bool active);
    Task PlayPlacementAsync(AppSettings settings);
}
