using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class LibraryRepository : Repository<Model.Library>
{
    public record LibrariesWithBooksCount(
        Guid Guid,
        string Name,
        User? Manager,
        int BooksCount
    );

    public LibraryRepository(LibraryContext db, string collectionName) : base(db, collectionName) {}

    public IReadOnlyList<LibrariesWithBooksCount> GetLibrariesWithBooksCount()
    {
        /*return _db.Libraries
            .Select(s => new LibrariesWithBooksCount(s.Guid, s.Name, s.Manager, s.Books.Count()))
            .ToList();*/
        return default;
    }
}