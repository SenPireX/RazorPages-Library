using Bogus;
using Library.Application.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

    public IMongoCollection<User> Users => _db.GetCollection<User>("users");
    public IMongoCollection<Model.Library> Libraries => _db.GetCollection<Model.Library>("libraries");
    public IMongoCollection<Book> Books => _db.GetCollection<Book>("books");
    public IMongoCollection<Loan> Loans => _db.GetCollection<Loan>("loans");
    public IMongoCollection<Member> Members => _db.GetCollection<Member>("members");
    public IMongoCollection<Librarian> Librarians => _db.GetCollection<Librarian>("librarians");

    public MongoClient GetClient() => _client;
    public IMongoDatabase GetDatabase() => _db;

    public void Seed(ICryptService cryptService)
    {
        Randomizer.Seed = new Random(1887);

        // Create admin user
        var adminSalt = cryptService.GenerateSecret(256);
        var admin = new User
        (
            username: "admin",
            salt: adminSalt,
            passwordHash: cryptService.GenerateHash(adminSalt, "1234"),
            usertype: Usertype.Admin
        );
        Users.InsertOne(admin);

        // Create members
        var members = new Faker<Member>("en").CustomInstantiator(f =>
                new Member(
                    firstName: f.Name.FirstName(),
                    lastName: f.Name.LastName(),
                    address: f.Address.FullAddress(),
                    email: f.Person.Email,
                    phoneNumber: f.Phone.PhoneNumber("+##-###-#######")
                ))
            .Generate(80)
            .ToList();
        Members.InsertMany(members);

        // Create librarians
        var librarians = new Faker<Librarian>("en").CustomInstantiator(f =>
                new Librarian(
                    firstName: f.Name.FirstName(),
                    lastName: f.Name.LastName(),
                    address: f.Address.FullAddress(),
                    email: f.Person.Email,
                    phoneNumber: f.Phone.PhoneNumber("+##-###-#######")
                ))
            .Generate(20)
            .ToList();
        Librarians.InsertMany(librarians);

        // Create libraries and books and loans
        var books = new List<Book>();
        var loans = new List<Loan>();
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
                    usertype: Usertype.Owner
                );
                Users.InsertOne(manager);

                var library = new Model.Library
                (
                    name: name,
                    books: [],
                    loans: [],
                    manager: manager
                );

                manager.Libraries.Add(library);

                var booksList = new Faker<Book>("en").CustomInstantiator(faker =>
                        new Book(
                            libraryGuid: library.Guid,
                            title: faker.Random.Words(2),
                            author: faker.Name.FullName(),
                            genre: faker.PickRandom<BookGenre>(),
                            publishedDate: faker.Date.PastDateOnly()
                        ))
                    .Generate(f.Random.Int(20, 50))
                    .ToList();

                books.AddRange(booksList);
                library.Books = booksList;

                var libraryLoans = new Faker<Loan>("en").CustomInstantiator(faker1 =>
                    {
                        var member = f.Random.ListItem(members);
                        var loanDate = faker1.Date.Recent();
                        var dueDate = loanDate.AddDays(14);
                        Book book;
                        
                        do
                        {
                            book = f.Random.ListItem(booksList);
                        } 
                        while (book.IsLoaned);
                        
                        book.IsLoaned = true;
                        Books.ReplaceOne(b => b.Guid == book.Guid, book);

                        var loan = new Loan
                        (
                            book: book,
                            library: library,
                            member: member,
                            loanDate: loanDate,
                            dueDate: dueDate
                        );

                        loans.Add(loan);
                        return loan;
                    })
                    .Generate(f.Random.Int(5, 10))
                    .ToList();

                library.Loans = libraryLoans;

                return library;
            })
            .Generate(10)
            .ToList();
        Libraries.InsertMany(libraries);
        Books.InsertMany(books);
        Loans.InsertMany(loans);

        // Create loans
        /*var loans = new Faker<Loan>("en").CustomInstantiator(f =>
            {
                var library = f.Random.ListItem(libraries);
                var member = f.Random.ListItem(members);
                var loanDate = DateTime.UtcNow;
                var dueDate = loanDate.AddDays(14);
                Book book;

                do
                {
                    book = f.Random.ListItem(books);
                }
                while (book.IsLoaned);

                book.IsLoaned = true;
                Books.ReplaceOne(b => b.Guid == book.Guid, book);

                var loan = new Loan(
                    book: book,
                    library: library,
                    member: member,
                    loanDate: loanDate,
                    dueDate: dueDate
                );

                var loansList = new Faker<Loan>("en").CustomInstantiator(faker =>
                        new Loan(
                            book: book,
                            library: library,
                            member: member,
                            loanDate: loanDate,
                            dueDate: dueDate
                        ))
                    .Generate(f.Random.Int(20, 50))
                    .ToList();

                library.Loans.Add(loan);
                library.Loans = loans;
                return loan;
            })
            .Generate(80)
            .ToList();
        Loans.InsertMany(loans);*/

        // Update libraries with new loans
        /* foreach (var library in libraries)
         {
             Libraries.ReplaceOne(l => l.Guid == library.Guid, library);
         }*/
    }
}