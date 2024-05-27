using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Loan //: IEntity<int>
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.ObjectId)] public string Id { get; private set; } // PK
    public Guid Guid { get; private set; }
    public string BookId { get; set; } // FK
    public string LibraryId { get; set; } // FK
    public string MemberId { get; set; } // FK
    [BsonIgnore] public Book Book { get; set; } // NAV
    [BsonIgnore] public Library Library { get; set; } // NAV
    [BsonIgnore] public Member Member { get; set; } // NAV
    public DateTime LoanDate { get; set; }
    public DateTime DueDate => LoanDate.AddDays(14);
    public DateTime? ReturnDate { get; set; }

    public Loan(Book book, Library library, Member member, DateTime loanDate)
    {
        Guid = Guid.NewGuid();
        Book = book;
        BookId = book.Id;
        LibraryId = library.Id;
        Library = library;
        MemberId = member.Id;
        Member = member;
        LoanDate = loanDate;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Loan() {}
    
}