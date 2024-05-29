using Library.Application.Model;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class UserRepository : Repository<User>
{
    private readonly ICryptService _cryptService;
    
    public UserRepository(LibraryContext db, ICryptService cryptService) : base(db, collectionName: "users")
    {
        _cryptService = cryptService;
    }
}