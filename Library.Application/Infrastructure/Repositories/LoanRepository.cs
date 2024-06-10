using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class LoanRepository : Repository<Loan>
{
    private readonly ILogger<Loan> _logger;

    public LoanRepository(LibraryContext db, ILogger<Loan> logger) : base(db, collectionName: "loans", logger)
    {
        _logger = logger;
    }
    
    public override (bool success, string message) Insert(Loan loan)
    {
        loan.LoanDate = DateTime.UtcNow;
        return base.Insert(loan);
    }

    public (bool success, string message) Insert(Guid bookGuid, Guid libraryGuid, Guid memberGuid, DateTime loanDate, DateTime dueDate)
    {
        var stringBookGuid = bookGuid.ToString();
        var bookFilter = Builders<Book>.Filter.Eq("_id", stringBookGuid);
        var book = _db.Books.Find(bookFilter).FirstOrDefault();
        if (book is null) { return (false, "Book is null."); }

        var stringLibraryGuid = libraryGuid.ToString();
        var libraryFilter = Builders<Model.Library>.Filter.Eq("_id", stringLibraryGuid);
        var library = _db.Libraries.Find(libraryFilter).FirstOrDefault();
        if (library is null) { return (false, "Library is null."); }

        var stringMemberGuid = memberGuid.ToString();
        var memberFilter = Builders<Member>.Filter.Eq("_id", stringMemberGuid);
        var member = _db.Members.Find(memberFilter).FirstOrDefault();
        if (member is null) { return (false, "Member is null."); }
        
        return base.Insert(new Loan(
            book: book,
            library: library,
            member: member,
            loanDate: loanDate,
            dueDate: dueDate
        ));
    }

    public IEnumerable<Loan> GetLoansByLibrary(Guid libraryGuid)
    {
        var stringLibraryGuid = libraryGuid.ToString();
        return _collection.Find(l => l.LibraryGuid.ToString() == stringLibraryGuid).ToList();
    }
    
}