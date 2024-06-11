using Bogus;
using Library.Application.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Library.Application.Infrastructure;

public class LibraryContext
{
    private readonly IMongoDatabase _db;
    private readonly MongoClient _client;

    public LibraryContext(IConfiguration config)
    {
        _client = new MongoClient(config.GetConnectionString("MongoDb"));
        _db = _client.GetDatabase("library");
    }

    public IMongoCollection<Model.Library> Libraries => _db.GetCollection<Model.Library>("libraries");
    public IMongoCollection<Book> Books => _db.GetCollection<Book>("books");
    public IMongoCollection<Loan> Loans => _db.GetCollection<Loan>("loans");
    public IMongoCollection<User> Users => _db.GetCollection<User>("users");
    public IMongoCollection<Member> Members => _db.GetCollection<Member>("members");

    //public IMongoCollection<Librarian> Librarians => _db.GetCollection<Librarian>("librarians");
    public MongoClient GetClient() => _client;
    public IMongoDatabase GetDatabase() => _db;

    public void Seed(ICryptService cryptService)
    {
        Randomizer.Seed = new Random(3661);

        var adminSalt = cryptService.GenerateSecret(256);
        var admin = new User
        (
            username: "admin",
            salt: adminSalt,
            passwordHash: cryptService.GenerateHash(adminSalt, "1234"),
            usertype: Usertype.Admin
        );
        Users.InsertOne(admin);

        var i = 0;
        var libraries = new Faker<Model.Library>("en").CustomInstantiator(f =>
            {
                var name = f.Address.City() + " City Library";
                var salt = cryptService.GenerateSecret(256);
                var username = $"library{++i:000}";
                var manager = new User
                (
                    username: username,
                    salt: salt,
                    passwordHash: cryptService.GenerateHash(key: salt, "1234"),
                    Usertype.Owner
                );
                Users.InsertOne(manager);

                var library = new Model.Library
                (
                    name: name,
                    books: new List<Book>(),
                    loans: new List<Loan>(),
                    manager: manager
                );
                return library;
            })
            .Generate(5)
            .ToList();

        Libraries.InsertMany(libraries);

        var books = new Faker<Book>("en").CustomInstantiator(f =>
                new Book
                (
                    libraryGuid: f.PickRandom(libraries).Guid,
                    title: f.Random.Words(2),
                    author: f.Name.FullName(),
                    genre: f.PickRandom<BookGenre>(),
                    publishedDate: f.Date.PastDateOnly()
                ))
            .Generate(100)
            .ToList();

        Books.InsertMany(books);

        foreach (var library in libraries)
        {
            library.Books = books.Where(book => book.LibraryGuid == library.Guid).ToList();
            Libraries.ReplaceOne(l => l.Guid == library.Guid, library);
        }

        var members = new Faker<Member>("en")
            .CustomInstantiator(f =>
                new Member
                (
                    firstName: f.Name.FirstName(),
                    lastName: f.Name.LastName(),
                    address: f.Address.FullAddress(),
                    email: f.Internet.Email(),
                    phoneNumber: f.Phone.PhoneNumber("+##-###-#######")
                ))
            .Generate(20)
            .ToList();

        Members.InsertMany(members);

        var loans = new Faker<Loan>("en")
            .CustomInstantiator(f =>
                new Loan
                (
                    book: f.PickRandom(books),
                    library: f.PickRandom(libraries),
                    member: f.PickRandom(members),
                    loanDate: f.Date.Recent(),
                    dueDate: DateTime.Now.AddDays(14)
                ))
            .Generate(100)
            .ToList();

        Loans.InsertMany(loans);

        foreach (var loan in loans)
        {
            var library = libraries.First(l => l.Guid == loan.LibraryGuid);
            var book = books.First(b => b.Guid == loan.BookGuid);
            book.IsLoaned = true;
            library.Loans.Add(loan);
            Books.ReplaceOne(b => b.Guid == book.Guid, book);
            Libraries.ReplaceOne(l => l.Guid == library.Guid, library);
        }
    }
}