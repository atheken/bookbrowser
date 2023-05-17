namespace BookBrowser.Models;

public partial class BooksPublishersLink
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long PublisherId { get; set; }
}
