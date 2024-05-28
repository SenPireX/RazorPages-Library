using Library.Application.Model;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class UserRepository
{
    private readonly ICryptService _cryptService;
    
    public UserRepository(LibraryContext db, string collectionName, ICryptService cryptService)
    {
        _cryptService = cryptService;
    }
}