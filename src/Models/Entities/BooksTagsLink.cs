namespace BookBrowser.Models;

public partial class BooksTagsLink
{
    public long Id { get; set; }

    public long Book { get; set; }

    public long Tag { get; set; }
}
