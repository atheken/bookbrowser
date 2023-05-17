namespace BookBrowser.Models;

public partial class BooksSeriesLink
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long SeriesId { get; set; }
}
