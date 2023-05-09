namespace BookBrowser.Models;

public partial class BookAuthorLink
{
    public long Id { get; set; }

    public long BookId { get; set; }
    
    public long AuthorId { get; set; }
}
