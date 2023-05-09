namespace BookBrowser.Models;

public partial class BooksSeriesLink
{
    public long Id { get; set; }

    public long Book { get; set; }

    public long Series { get; set; }
}
