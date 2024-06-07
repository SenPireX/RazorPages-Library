using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class BookRepository : Repository<Book>
{
    private readonly ILogger<Book> _logger;
    public BookRepository(LibraryContext db, ILogger<Book> logger) : base(db, collectionName: "books", logger)
    {
        _logger = logger;
    }

}