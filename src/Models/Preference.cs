namespace BookBrowser.Models;

public partial class Preference
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string Val { get; set; } = null!;
}
