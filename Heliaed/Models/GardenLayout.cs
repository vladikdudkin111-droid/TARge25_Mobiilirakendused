using System.Text.Json;

namespace Heliaed.Models;

public sealed class GardenLayout
{
    public const int Size = 6;
    public const int CellCount = Size * Size;
    public static readonly IReadOnlyList<string> PictureKinds = Array.AsReadOnly(new[]
        { "leaf", "flower", "moon", "sun", "tree", "mushroom", "butterfly", "bird", "cloud", "star" });
    public static readonly IReadOnlyList<string> Kinds = Array.AsReadOnly(PictureKinds.Append("empty").ToArray());
    private readonly string[] cells;
    private string[]? originalLargeCells;
    public IReadOnlyList<string> Cells => Array.AsReadOnly(cells);
    public GardenLayout() => cells = Enumerable.Repeat("empty", CellCount).ToArray();
    public void Place(int index, string kind)
    {
        if (index < 0 || index >= CellCount) throw new ArgumentOutOfRangeException(nameof(index));
        if (!Kinds.Contains(kind)) throw new ArgumentException("Tundmatu kujund.", nameof(kind));
        cells[index] = kind;
    }
    public string ToJson()
    {
        if (originalLargeCells is null) return JsonSerializer.Serialize(cells);
        // Vana suure aia nähtamatud ruudud säilivad ka uuesti salvestamisel.
        var preserved = (string[])originalLargeCells.Clone();
        for (int i = 0; i < CellCount; i++) preserved[(i / Size) * 10 + i % Size] = cells[i];
        return JsonSerializer.Serialize(preserved);
    }
    public static GardenLayout FromJson(string json)
    {
        var values = JsonSerializer.Deserialize<string[]>(json);
        if (values is null || (values.Length != CellCount && values.Length != 9 && values.Length != 100) || values.Any(x => !Kinds.Contains(x)))
            throw new FormatException("Paigutus peab sisaldama 6 × 6 välja või vana 3 × 3 või 10 × 10 aeda.");
        var layout = new GardenLayout();
        if (values.Length == 9)
        {
            // Vana aia read säilivad uue välja vasakus ülanurgas.
            for (int i = 0; i < values.Length; i++) layout.cells[(i / 3) * Size + i % 3] = values[i];
        }
        else if (values.Length == 100)
        {
            layout.originalLargeCells = values;
            for (int i = 0; i < CellCount; i++) layout.cells[i] = values[(i / Size) * 10 + i % Size];
        }
        else Array.Copy(values, layout.cells, CellCount);
        return layout;
    }
}
