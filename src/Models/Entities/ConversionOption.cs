namespace BookBrowser.Models;

public partial class ConversionOption
{
    public long Id { get; set; }

    public string Format { get; set; } = null!;

    public long? Book { get; set; }

    public byte[] Data { get; set; } = null!;
}
