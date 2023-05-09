namespace BookBrowser.Models;

public partial class BooksRatingsLink
{
    public long Id { get; set; }

    public long Book { get; set; }

    public long Rating { get; set; }
}
