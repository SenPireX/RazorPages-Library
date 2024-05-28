using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Loan
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.String)] public Guid Guid { get; private set; }
    public Guid BookId { get; set; } // FK
    public Guid LibraryId { get; set; } // FK
    public Guid MemberId { get; set; } // FK
    [BsonIgnore] public Book Book { get; set; } // NAV
    [BsonIgnore] public Library Library { get; set; } // NAV
    [BsonIgnore] public Member Member { get; set; } // NAV
    public DateTime LoanDate { get; set; }
    public DateTime DueDate => LoanDate.AddDays(14);
    public DateTime? ReturnDate { get; set; }

    public Loan(Book book, Library library, Member member)
    {
        Guid = Guid.NewGuid();
        Book = book;
        BookId = book.Guid;
        LibraryId = library.Guid;
        Library = library;
        MemberId = member.Guid;
        Member = member;
        LoanDate = DateTime.UtcNow;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Loan() {}
    
}