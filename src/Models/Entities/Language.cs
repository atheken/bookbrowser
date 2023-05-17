namespace BookBrowser.Models;

public partial class Language : IBookGrouping
{
    public long Id { get; set; }

    public string LangCode { get; set; } = null!;

    public string Link { get; set; } = null!;
    
    public List<Book> Books { get; set; } = new();
    
    public string Name => LangCode;
    
}
