namespace BookBrowser.Models;

public partial class BooksPluginDatum
{
    public long Id { get; set; }

    public long Book { get; set; }

    public string Name { get; set; } = null!;

    public string Val { get; set; } = null!;
}
