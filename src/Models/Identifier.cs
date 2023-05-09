namespace BookBrowser.Models;

public partial class Identifier
{
    public long Id { get; set; }

    public long Book { get; set; }

    public string Type { get; set; } = null!;

    public string Val { get; set; } = null!;
}
