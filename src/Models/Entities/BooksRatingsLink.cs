namespace BookBrowser.Models;

public partial class BooksRatingsLink
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long RatingId { get; set; }
}
