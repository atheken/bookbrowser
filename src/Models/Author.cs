namespace BookBrowser.Models;

public partial class Author
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Sort { get; set; }

    public string Link { get; set; } = null!;
}
