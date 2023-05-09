namespace BookBrowser.Models;

public partial class Datum
{
    public long Id { get; set; }

    public long Book { get; set; }

    public string Format { get; set; } = null!;

    public long UncompressedSize { get; set; }

    public string Name { get; set; } = null!;
}
