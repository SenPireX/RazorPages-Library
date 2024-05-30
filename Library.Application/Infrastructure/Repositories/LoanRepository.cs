using Library.Application.Model;
using Microsoft.Extensions.Logging;

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
    
    //TODO? public Insert
}