using Library.Application.Model;

namespace Library.Application.Infrastructure.Repositories;

public class LoanRepository : Repository<Loan>
{
    public LoanRepository(LibraryContext db) : base(db, collectionName: "loans") {}

    public override Task<(bool success, string message)> InsertAsync(Loan loan)
    {
        loan.LoanDate = DateTime.UtcNow;
        return base.InsertAsync(loan);
    }
    
    //TODO? public InsertAsync
}