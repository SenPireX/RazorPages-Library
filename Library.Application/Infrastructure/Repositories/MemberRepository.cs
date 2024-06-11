using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class MemberRepository : Repository<Member>
{
    public MemberRepository(LibraryContext db, ILogger<Member> logger) : base(db, "members", logger) {}
    
}