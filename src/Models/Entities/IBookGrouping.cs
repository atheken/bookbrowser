namespace BookBrowser.Models;

/// <summary>
/// A category by which books are grouped.
/// </summary>
public class IBookGrouping
{
    public long Id { get; }

    public string Name { get; }

    public List<Book> Books { get;  }
}