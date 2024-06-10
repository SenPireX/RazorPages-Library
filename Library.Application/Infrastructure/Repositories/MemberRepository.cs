using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class MemberRepository : Repository<Member>
{
    public MemberRepository(LibraryContext db, ILogger<Member> logger) : base(db, "members", logger)
    {
    }

    public Member GetMemberByGuid(Guid memberGuid)
    {
        var stringMemberGuid = memberGuid.ToString();
        return _collection.Find(m => m.Guid.ToString() == stringMemberGuid).FirstOrDefault();
    }
}