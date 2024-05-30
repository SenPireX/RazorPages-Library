using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class UserRepository : Repository<User>
{
    private readonly ICryptService _cryptService;
    private readonly ILogger<User> _logger;
    
    public UserRepository(LibraryContext db, ICryptService cryptService, ILogger<User> logger) : base(db, collectionName: "users", logger)
    {
        _cryptService = cryptService;
        _logger = logger;
    }
}