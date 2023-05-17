namespace BookBrowser.Models;

public class IBookGrouping
{
    public long Id { get; set; }

    public string Name { get; set; }

    public List<Book> Books { get; set; }
}