namespace BookBrowser.Models;

public partial class Rating : IBookGrouping
{
    public long Id { get; set; }

    public long RatingValue { get; set; }

    public string Link { get; set; } = null!;

    public List<Book> Books { get; set; } = new();

    public string Name => RatingValue.ToString();
}
