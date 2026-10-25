using Heliaed.Models;
using Plugin.Maui.Audio;

namespace Heliaed.Services;

public sealed class AudioService(IAudioManager manager) : IAudioService, IDisposable
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private IAudioPlayer? music, placement;
    private Stream? musicStream, placementStream;
    public async Task ApplyAsync(AppSettings settings, bool active)
    {
        await gate.WaitAsync();
        try
        {
            if (active && settings.SoundEnabled && settings.MusicPlaying)
            {
                if (music is null)
                {
                    musicStream = await FileSystem.OpenAppPackageFileAsync("garden.wav");
                    music = manager.CreatePlayer(musicStream);
                    music.Loop = true;
                }
                music.Volume = settings.Volume;
                if (!music.IsPlaying) music.Play();
            }
            else music?.Pause();
            if (!active || !settings.SoundEnabled) placement?.Stop();
            if (placement is not null) placement.Volume = settings.Volume;
        }
        finally { gate.Release(); }
    }
    public async Task PlayPlacementAsync(AppSettings settings)
    {
        if (!settings.SoundEnabled) return;
        await gate.WaitAsync();
        try
        {
            if (placement is null)
            {
                placementStream = await FileSystem.OpenAppPackageFileAsync("place.wav");
                placement = manager.CreatePlayer(placementStream);
            }
            placement.Stop();
            placement.Volume = settings.Volume;
            placement.Play();
        }
        finally { gate.Release(); }
    }
    public void Dispose()
    {
        music?.Stop(); placement?.Stop();
        music?.Dispose(); placement?.Dispose();
        musicStream?.Dispose(); placementStream?.Dispose();
    }
}
