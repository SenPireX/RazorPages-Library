using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Librarian //: IEntity<int>
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.ObjectId)] public string Id { get; set; }
    public Guid Guid { get; private set; }
    [MaxLength(16)] public string FirstName { get; set; }
    [MaxLength(16)] public string LastName { get; set; }
    [MaxLength(64)] public string Address { get; set; }
    [MaxLength(64)] public string Email { get; set; }
    [MaxLength(16)] public string PhoneNumber { get; set; }

    public Librarian(string firstName, string lastName, string address, string email, string phoneNumber)
    {
        Guid = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        Email = email;
        PhoneNumber = phoneNumber;
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Librarian() {}
}