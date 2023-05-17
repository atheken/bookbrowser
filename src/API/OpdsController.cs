using System.Collections.Immutable;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using BookBrowser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BookBrowser.API;

[ApiController]
[Route("opds")]
public class OpdsController : ControllerBase
{
    private readonly ContextFactory _dbFactory;
    private readonly ConfigurationOptions _options;
    private const int PageLimit = 50;

    public OpdsController(ContextFactory dbFactory, ConfigurationOptions options)
    {
        _dbFactory = dbFactory;
        _options = options;
    }

    [HttpGet("media/{bookId:long}/cover")]
    public async Task<IActionResult> Cover(long bookId)
    {
        await using var db = _dbFactory();
        var book = await db.Books.FindAsync(bookId);
        if (book != null)
        {
            var imagePath = Path.Combine(Path.GetFullPath(_options.CalibreLibraryPath), book.Path, "cover.jpg");
            if (Path.Exists(imagePath))
            {
                // generate the etag, and if the client already has it, respond with a 304 (Not Modified).
                var etag = imagePath.EtagForPath();
                if (Request.Headers["If-None-Match"].Contains(etag))
                {
                    return StatusCode(304);
                }
                
                Response.Headers.Add("Etag", etag);
                return new PhysicalFileResult(imagePath, "image/jpeg");
            }
        }

        return Redirect("/nocover.jpg");
    }
    
    [HttpGet("search-spec")]
    public async Task SearchSpec(string title = "")
    {
        var url = Url.Action(nameof(Books)) + "?search={searchTerms}";
        var ns = OpdsConstants.FeedNamespaces.Opensearch;
        var doc = new XDocument(new XElement(ns + "OpenSearchDescription",
            new XElement(ns + "LongName", $"Search {title}"),
            new XElement(ns + "ShortName", $"Search {title}"),
            new XElement(ns + "Url", new XAttribute("type", "application/atom+xml"),
                new XAttribute("template", url)),
            new XElement(ns + "SyndicationRight", "open"),
            new XElement(ns + "OutputEncoding", "UTF-8"),
            new XElement(ns + "InputEncoding", "UTF-8")
        ));
        Response.ContentType = "text/xml";
        var output = Response.BodyWriter.AsStream();

        await using var sr = new StreamWriter(output);
        await using var xmlWriter = new XmlTextWriter(sr);
        doc.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();
        await sr.FlushAsync();
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

        var book = await db.Books.Include(k => k.LibraryContents).FirstAsync(k => k.Id == id);
        var file = book.LibraryContents.First(k => k.Format.ToLower() == type).Name;
        var path = Path.Combine(_options.CalibreLibraryPath, book.Path, $"{file}.epub");
        return Path.Exists(path)
            ? new PhysicalFileResult(path,
                OpdsConstants.FormatTypes.GetValueOrDefault(type, "application/octet-stream"))
            : NotFound();
    }

    private static Dictionary<string, Func<IQueryable<Book>, IQueryable<Book>>> Sorts =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [""] = f => f.OrderBy(k => k.Sort),
            ["new"] = f => f.OrderByDescending(k => k.Timestamp)
        };

    /// <summary>
    /// List all books, or "new" books. "popularity" and featured/recommended may be supported in the future.
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="page"></param>
    [HttpGet("books/{sort?}")]
    public async Task Books(string sort = "", int page = 0,
        long? authorId = null, string? tag = null, long? seriesId = null, string search = "")
    {
        await using var db = _dbFactory();
        var baseQuery = db.Books.AsQueryable();
        var title = "All Books";

        if (authorId != null)
        {
            baseQuery = baseQuery.Where(k => k.Authors.Any(a => a.Id == authorId.Value));
            var author = await db.Authors.FindAsync(authorId);
            title = $"Books by `{author.Name}`";
        }
        else if (!string.IsNullOrWhiteSpace(tag))
        {
            baseQuery = baseQuery.Where(b => b.Tags.Any(k => k.Name.ToLower() == tag.ToLower()));
            title = $"Books tagged with `{tag}`";
        }
        else if (seriesId.HasValue)
        {
            baseQuery = db.Series.Where(s => s.Id == seriesId).SelectMany(k => k.Books);
            var series = await db.Series.FindAsync(seriesId);
            title = $"Books in Series: `{series.Name}";
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            baseQuery = baseQuery.Where(k =>
                k.Title.ToLower().Contains(search) || k.Authors.Any(a => a.Name.ToLower().Contains(search)));
        }

        var count = await baseQuery.CountAsync();

        var items = Sorts[sort](baseQuery)
            .Include(k => k.LibraryContents)
            .Include(k => k.Authors)
            .Include(k => k.Comment)
            .Skip(page * PageLimit).Take(PageLimit)
            .AsEnumerable()
            .Select(b => new SyndicationItem
            {
                Title = new TextSyndicationContent(b.Title),
                LastUpdatedTime = b.LastModified ?? DateTime.MinValue,
                Authors =
                {
                    b.Authors.Select(a =>
                        new SyndicationPerson(null, a.Name, Url.Action(nameof(Books), new {authorId = a.Id})))
                },
                Summary = !String.IsNullOrWhiteSpace(b.Comment?.Text)
                    ? new TextSyndicationContent(b.Comment.Text, TextSyndicationContentKind.Html)
                    : null,
                Links =
                {
                    new SyndicationLink
                    {
                        Uri = Url.Action(nameof(Cover), new {bookId = b.Id})!.AsUri(),
                        MediaType = "image/jpeg",
                        RelationshipType = OpdsConstants.Relations.Image
                    },
                    new SyndicationLink
                    {
                        Uri = Url.Action(nameof(Cover), new {bookId = b.Id})!.AsUri(),
                        MediaType = "image/jpeg",
                        RelationshipType = OpdsConstants.Relations.Thumbnail
                    },
                    b.LibraryContents.Select(f =>
                        new SyndicationLink
                        {
                            Uri = Url.Action(nameof(Acquire), new {b.Id, type = f.Format.ToLower()})!.AsUri(),
                            MediaType = OpdsConstants.FormatTypes.GetValueOrDefault(f.Format,
                                "application/octet-stream"),
                            RelationshipType = OpdsConstants.FeedTypes.Acquisition
                        })
                }
            });

        var a = items.ToArray();

        var feed = new SyndicationFeed(a)
        {
            Title = new TextSyndicationContent(title),
            Links =
            {
                new SyndicationLink
                {
                    Uri = Url.Action(nameof(SearchSpec)).AsUri(),
                    MediaType = OpdsConstants.FeedTypes.Search,
                    RelationshipType = OpdsConstants.Relations.Search
                }
            },
            ElementExtensions =
            {
                new XElement(OpdsConstants.FeedNamespaces.Opds + "pageLimit", PageLimit)
            }
        };

        AppendCrawlableLinks(feed, count, page, PageLimit);
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
            Links =
            {
                new SyndicationLink
                {
                    Title = "All Books",
                    MediaType = OpdsConstants.FeedTypes.Acquisition,
                    Uri = Url.Action(nameof(Books))!.AsPublicUri(Request),
                    RelationshipType = OpdsConstants.Relations.New
                }
            }
        };

        var newBooks = new SyndicationItem
        {
            Title = new TextSyndicationContent("New Books"),
            Content = new TextSyndicationContent("Recently added books."),
            LastUpdatedTime = lastUpdate,
            Links =
            {
                new SyndicationLink
                {
                    Title = "New Books",
                    MediaType = OpdsConstants.FeedTypes.Acquisition,
                    Uri = Url.Action(nameof(Books), new {sort = "new"})!.AsUri(),
                    RelationshipType = OpdsConstants.Relations.New
                }
            }
        };


        var feed = new SyndicationFeed(new[]
        {
            allBooks, newBooks
        }.Concat(Groupings.Select(k => new SyndicationItem
        {
            Title = new TextSyndicationContent(k.Value.Title),
            LastUpdatedTime = lastUpdate,
            Links =
            {
                new SyndicationLink
                {
                    Title = k.Value.Title,
                    MediaType = OpdsConstants.FeedTypes.Acquisition,
                    Uri = Url.Action(nameof(ListingByGrouping), new {grouping = k.Value.Grouping})!.AsUri(),
                    RelationshipType = OpdsConstants.Relations.Subsection
                }
            }
        })))
        {
            Links =
            {
                new SyndicationLink
                {
                    Uri = Url.Action(nameof(SearchSpec)).AsUri(),
                    MediaType = OpdsConstants.FeedTypes.Search,
                    RelationshipType = OpdsConstants.Relations.Search
                }
            }
        };

        WriteAtomToResponse(feed);
    }

    private static readonly IDictionary<string, GroupingDefinition> Groupings = new[]
    {
        new GroupingDefinition("author", "Books by Author", f => f.Authors, a=> new {authorId = a.Id}),
        new GroupingDefinition("tag", "Books by Popular Tags", f => f.Tags.Where(k=>k.Books.Count() >= 10), t=> new {tag = t.Name}),
        new GroupingDefinition("unique-tag", "Books by Unique Tags", f => f.Tags.Where(k=>k.Books.Count() < 10), t=> new {tag = t.Name}),
        new GroupingDefinition("series", "Books by Series", f => f.Series, t=> new {seriesId = t.Id}),
    }.ToImmutableDictionary(l => l.Grouping, v => v);

    [HttpGet("by-{grouping}")]
    public async Task ListingByGrouping(string grouping)
    {
        await using var db = _dbFactory();
        var group = Groupings[grouping.ToLower()];
        var groups = await group.BaseSelector(db).Select(k => new GroupingSummary(k.Name, k.Id, k.Books.Count()))
            .ToListAsync();

        var feed = new SyndicationFeed(groups.Select(f =>
            new SyndicationItem
            {
                Title = new TextSyndicationContent($"{f.Name} ({f.Count})"),
                Links =
                {
                    new SyndicationLink
                    {
                        Title = $"{f.Name} ({f.Count} works)",
                        MediaType = OpdsConstants.FeedTypes.Acquisition,
                        Uri = Url.Action(nameof(Books), group.FeedFilterSelector(f))!.AsUri(),
                        RelationshipType = OpdsConstants.Relations.Subsection
                    }
                }
            }))
        {
            Title = new TextSyndicationContent(group.Title)
        };

        WriteAtomToResponse(feed);
    }

    private SyndicationLink CreateCrawlableLink(int page, string relationship)
    {
        var routeClone = Request.RouteValues.ToDictionary(k => k.Key, v => v.Value);
        routeClone["page"] = page;
        return new SyndicationLink(Url
            .Action(routeClone["action"].ToString(), routeClone["controller"].ToString(), routeClone).AsUri())
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
    private void AppendCrawlableLinks(SyndicationFeed feed, int count, int current, int pageLimit)
    {
        var maxPage = (int) Math.Floor((float) count / pageLimit);
        feed.Links.Add(CreateCrawlableLink(current, OpdsConstants.Relations.Self));
        feed.Links.Add(CreateCrawlableLink(0, OpdsConstants.Relations.First));
        feed.Links.Add(CreateCrawlableLink(maxPage, OpdsConstants.Relations.Last));
        if (current + 1 <= maxPage)
        {
            feed.Links.Add(CreateCrawlableLink(current + 1, OpdsConstants.Relations.Next));
        }

        if (current - 1 >= 0)
        {
            feed.Links.Add(CreateCrawlableLink(current - 1, OpdsConstants.Relations.Previous));
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

record GroupingDefinition(string Grouping, string Title,
    Func<CalibreDbContext, IQueryable<IBookGrouping>> BaseSelector, Func<GroupingSummary, object> FeedFilterSelector);
    
record GroupingSummary(string Name, long Id, int Count);