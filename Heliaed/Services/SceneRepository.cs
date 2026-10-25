using SQLite;
using Heliaed.Models;

namespace Heliaed.Services;

public sealed class SceneRepository(string databasePath) : ISceneRepository, IAsyncDisposable
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private SQLiteAsyncConnection? connection;
    private async Task<SQLiteAsyncConnection> OpenAsync()
    {
        if (connection is not null) return connection;
        var db = new SQLiteAsyncConnection(databasePath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        await db.CreateTableAsync<GardenScene>();
        connection = db;
        return db;
    }
    public async Task<List<GardenScene>> GetAllAsync()
    {
        await gate.WaitAsync();
        try { return await (await OpenAsync()).Table<GardenScene>().OrderByDescending(x => x.UpdatedUtc).ToListAsync(); }
        finally { gate.Release(); }
    }
    public async Task SaveAsync(GardenScene scene)
    {
        var title = scene.Title.Trim();
        if (title.Length is < 1 or > 60) throw new ArgumentException("Nime pikkus peab olema 1–60 märki.");
        GardenLayout.FromJson(scene.LayoutJson);
        await gate.WaitAsync();
        try
        {
            var db = await OpenAsync();
            scene.Title = title;
            scene.UpdatedUtc = DateTime.UtcNow;
            if (scene.Id == 0) await db.InsertAsync(scene);
            else if (await db.UpdateAsync(scene) != 1) throw new InvalidOperationException("Kirjet ei leitud.");
        }
        finally { gate.Release(); }
    }
    public async Task DeleteAsync(int id)
    {
        await gate.WaitAsync();
        try { await (await OpenAsync()).DeleteAsync<GardenScene>(id); }
        finally { gate.Release(); }
    }
    public async ValueTask DisposeAsync()
    {
        await gate.WaitAsync();
        try { if (connection is not null) { await connection.CloseAsync(); connection = null; } }
        finally { gate.Release(); }
    }
}
