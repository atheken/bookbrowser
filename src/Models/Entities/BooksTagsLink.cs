namespace BookBrowser.Models;

public partial class BooksTagsLink
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long TagId { get; set; }
}
