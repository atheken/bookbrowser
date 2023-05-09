namespace BookBrowser.Models;

public partial class Language
{
    public long Id { get; set; }

    public string LangCode { get; set; } = null!;

    public string Link { get; set; } = null!;
}
