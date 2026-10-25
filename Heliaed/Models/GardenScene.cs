using SQLite;

namespace Heliaed.Models;

// Üks kirje hoiab nime ja aia paigutust; nähtav väli on 6 × 6.
[Table("GardenScenes")]
public sealed class GardenScene
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [MaxLength(60), NotNull] public string Title { get; set; } = "";
    [NotNull] public string LayoutJson { get; set; } = "[]";
    public DateTime UpdatedUtc { get; set; }
    [Ignore] public string Summary => $"#{Id} · {UpdatedUtc.ToLocalTime():dd.MM.yyyy HH:mm}";
}
