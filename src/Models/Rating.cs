namespace BookBrowser.Models;

public partial class Rating
{
    public long Id { get; set; }

    public long? Rating1 { get; set; }

    public string Link { get; set; } = null!;
}
