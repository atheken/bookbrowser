namespace BookBrowser.Models;

public partial class Book
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Sort { get; set; }

    public DateTime? Timestamp { get; set; }

    public DateTime? Pubdate { get; set; }

    public double SeriesIndex { get; set; }

    public string? AuthorSort { get; set; }

    public string? Isbn { get; set; }

    public string? Lccn { get; set; }

    public string Path { get; set; } = null!;

    public long Flags { get; set; }

    public string? Uuid { get; set; }

    public bool? HasCover { get; set; }

    public DateTime? LastModified { get; set; }

    public List<Author> Authors { get; set; } = new();

    /// <summary>
    /// The library content (list of ebooks stored on disk).
    /// </summary>
    public List<LibraryContent> LibraryContents = new ();
    
    public Comment? Comment { get; set; }

    public List<Tag> Tags { get; set; } = new();
}
