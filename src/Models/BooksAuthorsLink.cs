namespace BookBrowser.Models;

public partial class BooksAuthorsLink
{
    public long Id { get; set; }

    public long Book { get; set; }

    public long Author { get; set; }
}
