namespace BookBrowser.Models;

public partial class Tag : IBookGrouping
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Link { get; set; } = null!;
    
    public List<Book> Books { get; set; } = new();
}
