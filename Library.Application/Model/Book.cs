using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Application.Model;

public class Book
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [BsonId] [BsonRepresentation(BsonType.String)] public Guid Guid { get; private set; }
    [MaxLength(64)] public string Title { get; set; }
    [MaxLength(32)] public string Author { get; set; }
    public BookGenre Genre { get; set; }
    public DateOnly PublishedDate { get; set; }
    public bool IsLoaned { get; set; }
    
    public Book(string title, string author, BookGenre genre, DateOnly publishedDate)
    {
        Guid = Guid.NewGuid();
        Title = title;
        Author = author;
        Genre = genre;
        PublishedDate = publishedDate;
        IsLoaned = false;
    }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Book() {}

}