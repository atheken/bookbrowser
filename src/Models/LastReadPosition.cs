namespace BookBrowser.Models;

public partial class LastReadPosition
{
    public long Id { get; set; }

    public long Book { get; set; }

    public string Format { get; set; } = null!;

    public string User { get; set; } = null!;

    public string Device { get; set; } = null!;

    public string Cfi { get; set; } = null!;

    public double Epoch { get; set; }

    public double PosFrac { get; set; }
}
