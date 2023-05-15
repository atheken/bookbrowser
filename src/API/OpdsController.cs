using System.IO.Pipelines;
using System.IO.Pipes;
using System.Linq.Expressions;
using System.ServiceModel.Syndication;
using System.Xml;
using BookBrowser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookBrowser.API;

[ApiController]
[Route("opds")]
public class OpdsController:ControllerBase
{
    private readonly ContextFactory _dbFactory;
    private readonly ConfigurationOptions _options;
    private readonly int _pagelimit = 50;
    
    public OpdsController(ContextFactory dbFactory, ConfigurationOptions options)
    {
        _dbFactory = dbFactory;
        _options = options;
    }

    /// <summary>
    /// Allow downloading of the specified book.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet("acquire/book/{id}")]
    public async Task<IActionResult> Acquire(long id, string type = "epub")
    {
        await using var db = _dbFactory();
        
        var book = await db.Books.Include(k=>k.LibraryContents).FirstAsync(k=>k.Id == id);
        var file = book.LibraryContents.First(k => k.Format.ToLower() == type).Name;
        var path = Path.Combine(_options.CalibreLibraryPath, book.Path, $"{file}.epub");
        
        return new PhysicalFileResult(path, OpdsConstants.MediaTypes.Epub);
    }

    private static Dictionary<string, Func<IQueryable<Book>, IQueryable<Book>>> Sorts =
        new (StringComparer.InvariantCultureIgnoreCase)
        {
            [""] = f=> f.OrderBy(k=>k.Sort),
            ["new"] = f=> f.OrderByDescending( k => k.Timestamp)
        };

    /// <summary>
    /// List all books, or "new" books. "popularity" and featured/recommended may be supported in the future.
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="page"></param>
    [HttpGet("books/{sort?}")]
    public async Task Books(string sort = "", int page = 0)
    {
        await using var db = _dbFactory();
        var count = await db.Books.CountAsync();

        var items = Sorts[sort](db.Books)
            .Skip(page * _pagelimit).Take(_pagelimit)
            .AsEnumerable()
            .Select(b => new SyndicationItem
        {
            Title = new TextSyndicationContent(b.Title),
            LastUpdatedTime = b.LastModified ?? DateTime.MinValue,
            Links =
            {
                new SyndicationLink
                {
                 Uri  = new Uri($"/api/books/{b.Id}/cover/", UriKind.Relative),
                 MediaType = "image/jpeg",
                 RelationshipType = OpdsConstants.Relations.Image
                },
                new SyndicationLink
                {
                    Uri  = new Uri($"/opds/acquire/book/{b.Id}", UriKind.Relative),
                    MediaType = OpdsConstants.MediaTypes.Epub,
                    RelationshipType = OpdsConstants.FeedTypes.Acquisition
                }
            }
        });

        var a = items.ToArray();
        
        var feed = new SyndicationFeed(a);

        AppendCrawlableLinks(feed, count, page, _pagelimit, Request.Path);
        WriteAtomToResponse(feed);
    }

    /// <summary>
    /// Produce the root catalog listings.
    /// </summary>
    [HttpGet]
    public async Task Root()
    {
        await using var db = _dbFactory();
        var lastUpdate = await db.Books.MaxAsync(k => k.LastModified) ?? DateTime.MinValue;
        
        var allBooks = new SyndicationItem
        {
            Title = new TextSyndicationContent("All Books"),
            LastUpdatedTime = lastUpdate,
            Content = new TextSyndicationContent("All available books"),
            Links = {  new SyndicationLink
                {
                    Title = "All Books",
                    MediaType = OpdsConstants.FeedTypes.Acquisition,
                    Uri = new Uri("/opds/books/", UriKind.Relative),
                    RelationshipType = OpdsConstants.Relations.New
                }
                
            }
        };
        
        var newBooks = new SyndicationItem
        {
            Title = new TextSyndicationContent("New Books"),
            Content = new TextSyndicationContent("Recently added books."),
            LastUpdatedTime = lastUpdate,
            Links = {  new SyndicationLink
            {
                Title = "New Books",
                MediaType = OpdsConstants.FeedTypes.Acquisition,
                Uri = new Uri("/opds/books/new", UriKind.Relative),
                RelationshipType = OpdsConstants.Relations.New
            }
                
            }
        };

        WriteAtomToResponse(new SyndicationFeed(new[] {allBooks, newBooks}));
    }

    
    private SyndicationLink CreateCrawlableLink(string basepath, int page, string relationship)
    {
        return new SyndicationLink(new Uri($"{basepath}?page={page}", UriKind.Relative))
        {
            RelationshipType = relationship,
            MediaType = OpdsConstants.Relations.Crawlable
        };
    }
    
    /// <summary>
    /// Append links to support crawlable features
    /// </summary>
    /// <remarks>This currently wipes out any existing querystrings, so it will be useful to use a smarter implementation
    /// to support paging on search and tag feeds in the future.</remarks>
    /// <param name="feed"></param>
    /// <param name="count"></param>
    /// <param name="current"></param>
    /// <param name="pageLimit"></param>
    /// <param name="basePath"></param>
    private void AppendCrawlableLinks(SyndicationFeed feed, int count, int current, int pageLimit, string basePath)
    {  
        var maxPage = (int)Math.Floor((float)count / pageLimit);
        feed.Links.Add(CreateCrawlableLink(basePath, current, OpdsConstants.Relations.Self));
        feed.Links.Add(CreateCrawlableLink(basePath, 0, OpdsConstants.Relations.Start));
        feed.Links.Add(CreateCrawlableLink(basePath, maxPage, OpdsConstants.Relations.Last));
        if (current + 1 <= maxPage)
        {
            feed.Links.Add(CreateCrawlableLink(basePath, current+1, OpdsConstants.Relations.Next));
        }
        if (current - 1 >= 0)
        {
            feed.Links.Add(CreateCrawlableLink(basePath, current-1, OpdsConstants.Relations.Previous));
        }
    }

    /// <summary>
    /// Write the opds feed to the response stream.
    /// </summary>
    /// <param name="feed"></param>
    private async void WriteAtomToResponse(SyndicationFeed feed)
    {
        Response.ContentType = "application/atom+xml";
        var stream = Response.BodyWriter.AsStream();
        
        await using var streamWriter = new StreamWriter(stream);
        await using var xmlWriter = new XmlTextWriter(streamWriter);
        
        feed.GetAtom10Formatter().WriteTo(xmlWriter);
        xmlWriter.Flush();
        await streamWriter.FlushAsync();
    }
}