namespace BookBrowser;

/// <summary>
/// A delegate function that allows for the creation of a dbcontext.
/// Passing true will open a read connection, while passing false will open a readonly connection.
/// </summary>
public delegate CalibreDbContext ContextFactory(bool requiresWrite = false);

public class ConfigurationOptions
{
    private string _path = Path.GetFullPath("/books");

    /// <summary>
    /// The root directory of the calibre database/library.
    /// </summary>
    /// <remarks>This path can be absolute or relative,
    /// but will be normalized to the absolute path.</remarks>
    public string CalibreLibraryPath
    {
        get => _path;
        set => _path = Path.GetFullPath(value);
    }

    /// <summary>
    /// Configures the listening port.
    /// </summary>
    public int Port { get; set; } = 7777;
}

public static class Helpers
{
    /// <summary>
    /// Converts a string to a uri.
    /// </summary>
    /// <param name="uri">The string representation of the uri.</param>
    /// <param name="kind">The kind of URI to create.</param>
    /// <returns></returns>
    public static Uri AsUri(this string uri, UriKind kind = UriKind.RelativeOrAbsolute) => new (uri, kind);

    public static Uri AsPublicUri(this string pathAndQuery, HttpRequest requestContext) =>
        new ($"{requestContext.Scheme}://{requestContext.Host}{pathAndQuery}");
    
    /// <summary>
    /// This allows for spread of items when initializing a collection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items) {
        foreach (T item in items) {
            collection.Add(item);
        }
    }
}