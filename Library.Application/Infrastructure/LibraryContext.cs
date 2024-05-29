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
        var libraries = new Faker<Model.Library>("de").CustomInstantiator(f =>
            {
                var name = f.Address.City() + " City Library";
                var salt = cryptService.GenerateSecret(256);
                var username = $"library{++i:000}";
                return new Model.Library
                (
                    name: name,
                    member: new User
                    (
                        username: username,
                        salt: salt,
                        passwordHash: cryptService.GenerateHash(key: salt, "1234"),
                        usertype: Usertype.Owner
                    )
                );
            })
            .Generate(10)
            .ToList();
        Libraries.InsertMany(libraries);

        var books = new Faker<Book>("de").CustomInstantiator(f => new Book(
                title: f.Random.Words(2),
                author: f.Name.FullName(),
                genre: f.PickRandom<BookGenre>(),
                publishedDate: f.Date.PastDateOnly()
            ))
            .Generate(200)
            .ToList();
        Books.InsertMany(books);

        var members = new Faker<Member>("de").CustomInstantiator(f => new Member(
                firstName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                address: f.Address.FullAddress(),
                email: f.Person.Email,
                phoneNumber: f.Phone.PhoneNumber("+##-###-#######")
            ))
            .Generate(80)
            .ToList();
        Members.InsertMany(members);

        var librarians = new Faker<Librarian>("de").CustomInstantiator(f => new Librarian(
                firstName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                address: f.Address.FullAddress(),
                email: f.Person.Email,
                phoneNumber: f.Phone.PhoneNumber("+##-###-#######")
            ))
            .Generate(20)
            .ToList();
        Librarians.InsertMany(librarians);

        var loans = new Faker<Loan>("de").CustomInstantiator(f =>
            {
                var library = f.Random.ListItem(libraries);
                var book = f.Random.ListItem(books);
                var member = f.Random.ListItem(members);

                return new Loan(
                    book: book,
                    library: library,
                    member: member
                );
            })
            .Generate(80)
            .ToList();
        Loans.InsertMany(loans);
    }
}

/*public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions opt) : base(opt) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Model.Library> Libraries => Set<Model.Library>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Librarian> Librarians => Set<Librarian>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>().ToTable("Loan");
        modelBuilder.Entity<Loan>().HasIndex(l => new { l.LibraryId, l.BookId }).IsUnique();
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var type = entity.ClrType;
            if (type.GetProperty("Guid") is not null)
                modelBuilder.Entity(type).HasAlternateKey("Guid");
        }
    }

    public void Seed(ICryptService cryptService)
    {
        Randomizer.Seed = new Random(1938);

        var adminSalt = cryptService.GenerateSecret(256);
        var admin = new User(
            username: "admin",
            salt: adminSalt,
            passwordHash: cryptService.GenerateHash(adminSalt, "1234"),
            usertype: Usertype.Admin);
        Users.Add(admin);
        SaveChanges();

        var i = 0;
        var libraries = new Faker<Model.Library>("de").CustomInstantiator(f =>
            {
                var name = f.Company.CompanyName();
                var salt = cryptService.GenerateSecret(256);
                var username = $"library{++i:000}";
                return new Model.Library
                (
                    name: f.Company.CompanyName(),
                    url: f.Internet.Url().OrDefault(f, 0.25f),
                    manager: new User
                    (
                        username: username,
                        salt: salt,
                        passwordHash: cryptService.GenerateHash(key: salt, "1234"),
                        usertype: Usertype.Owner
                    )
                );
            })
            .Generate(10)
            .GroupBy(l => l.Name).Select(g => g.First())
            .ToList();
        Libraries.AddRange(libraries);
        SaveChanges();

        var books = new Faker<Book>("de").CustomInstantiator(f => new Book(
                title: f.Random.Words(2),
                author: f.Person.FullName,
                genre: f.Random.Words(),
                publishedDate: f.Date.Past()
            ))
            .Generate(200)
            .GroupBy(b => b.Id).Select(g => g.First())
            .ToList();
        Books.AddRange(books);
        SaveChanges();

        var members = new Faker<Member>("de").CustomInstantiator(f => new Member(
                firstName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                address: f.Address.FullAddress(),
                email: f.Person.Email,
                phoneNumber: f.Phone.PhoneNumber()
            ))
            .Generate(120)
            .GroupBy(m => m.Id).Select(g => g.First())
            .ToList();
        Members.AddRange(members);
        SaveChanges();

        var librarians = new Faker<Librarian>("de").CustomInstantiator(f => new Librarian(
                firstName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                address: f.Address.FullAddress(),
                email: f.Person.Email,
                phoneNumber: f.Phone.PhoneNumber()
            ))
            .Generate(20)
            .GroupBy(m => m.Id).Select(g => g.First())
            .ToList();
        Librarians.AddRange(librarians);
        SaveChanges();

        var loans = new Faker<Loan>("de").CustomInstantiator(f =>
            {
                var library = f.Random.ListItem(libraries);
                var book = f.Random.ListItem(books);
                var member = f.Random.ListItem(members);

                return new Loan(
                    book: book,
                    library: library,
                    member: member,
                    loanDate: DateTime.UtcNow
                );
            })
            .Generate(80)
            .GroupBy(l => new { l.LibraryId, l.BookId }).Select(g => g.First())
            .ToList();
        Loans.AddRange(loans);
        SaveChanges();
    }
}*/