using Microsoft.AspNetCore.Mvc;
namespace BookBrowser.API;

[ApiController]
[Route("api/books")]
public class BooksController:ControllerBase
{
    private readonly ContextFactory _dbFactory;
    private readonly ConfigurationOptions _config;

    public BooksController(ContextFactory dbFactory, ConfigurationOptions options)
    {
        _dbFactory = dbFactory;
        _config = options;
    }
    
    [HttpGet("{bookId:long}/cover")]
    public async Task<IActionResult> Cover(long bookId)
    {
        await using var db = _dbFactory();
        var book = await db.Books.FindAsync(bookId);
        if (book != null)
        {
            var imagePath = Path.Combine(_config.CalibreLibraryPath, book.Path, "cover.jpg");
            if (Path.Exists(imagePath))
            {
                return new PhysicalFileResult(imagePath, "image/jpeg");
            }
        }

        return Redirect("/nocover.jpg");
    }
}