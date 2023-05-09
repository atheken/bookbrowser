using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BookBrowser.Models;

public class BookListingService
{
    private readonly ContextFactory _contextFactory;

    public BookListingService(ContextFactory dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    /// <summary>
    /// Get a page of books and the total number of books.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="orderBy">The column onto which we should sort. If not specified, we use the `sort` column.</param>
    /// <param name="sortAscending">Should the sort be ascending or descending.</param>
    /// <returns></returns>
    public async Task<(int count, IEnumerable<Book> books)> GetBooks(int offset = 0, int limit = 20, Expression<Func<Book,object>>? orderBy = null, bool sortAscending = true)
    {
        
        await using var db = _contextFactory();
        
        var count = await db.Books.CountAsync();

        orderBy ??= k => k.Sort;
        
        var baseQuery = sortAscending ? db.Books.OrderBy(orderBy) : db.Books.OrderByDescending(orderBy);
        var books = await baseQuery.Take(limit).Skip(offset).ToArrayAsync();
        
        return new(count, books);
    }
}
