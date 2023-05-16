using System.Collections.Immutable;
using BookBrowser.Models;
using BookBrowser.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookBrowser.API;

[ApiController]
[Route("api/{controller}")]
public class BooksController:ControllerBase
{
    private readonly ContextFactory _dbFactory;
    private readonly ConfigurationOptions _config;

    public BooksController(ContextFactory dbFactory, ConfigurationOptions options)
    {
        _dbFactory = dbFactory;
        _config = options;
    }
    
    [HttpGet]
    public async Task<ResultPageView<Book>> Get(int limit = 20, int offset = 0, bool sortAscending = true){
        
        await using var db = _dbFactory();
        var count = db.Books.CountAsync();

        var baseQuery = sortAscending ? db.Books.OrderBy(k => k.Sort) : db.Books.OrderByDescending(k=> k.Sort);
        var books = baseQuery.Include(k=>k.Authors).Skip(offset).Take(limit).ToListAsync();
            //.Select(k=> new SimpleBookView(k.Id, k.Title, k.Authors.Select(a=> new SimpleAuthorView( a.Name, a.Id)).ToImmutableList())).ToListAsync();

        return new (await count, await books);
    }

    [HttpGet("{bookId:long}/cover")]
    public async Task<IActionResult> Cover(long bookId)
    {
        await using var db = _dbFactory();
        var book = await db.Books.FindAsync(bookId);
        if (book != null)
        {
            var imagePath = Path.Combine(Path.GetFullPath(_config.CalibreLibraryPath), book.Path, "cover.jpg");
            if (Path.Exists(imagePath))
            {
                return new PhysicalFileResult(imagePath, "image/jpeg");
            }
        }

        return Redirect("/nocover.jpg");
    }
}