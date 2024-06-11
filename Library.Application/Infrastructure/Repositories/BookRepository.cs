using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class BookRepository : Repository<Book>
{
    public BookRepository(LibraryContext db, ILogger<Book> logger) : base(db, collectionName: "books", logger) { }

    public IEnumerable<Book> GetAvailableBooks(Guid libraryGuid)
    {
        var stringLibraryGuid = libraryGuid.ToString();
        return _collection.Find(b => b.LibraryGuid.ToString() == stringLibraryGuid && !b.IsLoaned).ToList();
    }
    
}