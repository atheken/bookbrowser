namespace BookBrowser.Models;

public partial class Tag
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Link { get; set; } = null!;
}
