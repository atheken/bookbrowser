namespace BookBrowser.Utilities;

/// <summary>
/// A delegate function that allows for the creation of a dbcontext.
/// Passing true will open a read connection, while passing false will open a readonly connection.
/// </summary>
public delegate MetadataContext? ContextFactory(bool requiresWrite = false);

public class ConfigurationOptions
{
    /// <summary>
    /// The root directory of the calibre database/library.
    /// </summary>
    public string CalibreLibraryPath { get; set; }

    /// <summary>
    /// Configures the listening port.
    /// </summary>
    public int Port { get; set; } = 7777;
}