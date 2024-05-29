using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class BookRepository : Repository<Book>
{
    public BookRepository(LibraryContext db) : base(db, collectionName: "books")
    {}
    
}