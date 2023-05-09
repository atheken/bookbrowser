namespace BookBrowser.Models;

public partial class BooksPublishersLink
{
    public long Id { get; set; }

    public long Book { get; set; }

    public long Publisher { get; set; }
}
