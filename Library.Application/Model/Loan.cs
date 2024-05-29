using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Loan
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.String)] public Guid Guid { get; private set; }
    [BsonRepresentation(BsonType.String)] public Guid BookGuid { get; set; }
    [BsonRepresentation(BsonType.String)] public Guid LibraryGuid { get; set; }
    [BsonRepresentation(BsonType.String)] public Guid MemberGuid { get; set; }
    [BsonIgnore] public Book Book { get; set; }
    [BsonIgnore] public Library Library { get; set; }
    [BsonIgnore] public Member Member { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Loan(Book book, Library library, Member member)
    {
        Guid = Guid.NewGuid();
        Book = book;
        BookGuid = book.Guid;
        LibraryGuid = library.Guid;
        Library = library;
        MemberGuid = member.Guid;
        Member = member;
        LoanDate = DateTime.UtcNow;
        DueDate = LoanDate.AddDays(14);
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Loan() {}
    
}