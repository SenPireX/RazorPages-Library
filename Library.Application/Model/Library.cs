using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Library //: IEntity<int>
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.ObjectId)] public string Id { get; private set; }
    public Guid Guid { get; private set; }
    [MaxLength(32)] public string Name { get; set; }
    [MaxLength(255)] public string? Url { get; set; }
    public User? Manager { get; set; }
    
    public Library(string name, string? url = null, User? manager = null)
    {
        Guid = Guid.NewGuid();
        Name = name;
        Url = url;
        Manager = manager;
    }
    
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Library() {}

}