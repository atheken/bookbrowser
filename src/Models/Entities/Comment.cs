namespace BookBrowser.Models;

public partial class Comment
{
    public long Id { get; set; }

    public long BookId { get; set; }

    public string Text { get; set; } = null!;
    
}
