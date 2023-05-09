namespace BookBrowser.Models;

public partial class CustomColumn
{
    public long Id { get; set; }

    public string Label { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Datatype { get; set; } = null!;

    public byte[] MarkForDelete { get; set; } = null!;

    public byte[] Editable { get; set; } = null!;

    public string Display { get; set; } = null!;

    public byte[] IsMultiple { get; set; } = null!;

    public byte[] Normalized { get; set; } = null!;
}
