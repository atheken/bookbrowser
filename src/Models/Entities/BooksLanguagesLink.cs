namespace BookBrowser.Models;

public partial class BooksLanguagesLink
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public long LanguageId { get; set; }

    public long ItemOrder { get; set; }
}
