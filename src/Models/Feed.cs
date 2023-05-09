namespace BookBrowser.Models;

public partial class Feed
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Script { get; set; } = null!;
}
