using Library.Application.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class LibraryRepository : Repository<Model.Library>
{
    public record LibrariesWithBooksCount(
        Guid Guid,
        string Name,
        User? Member,
        int BooksCount
    );

    public LibraryRepository(LibraryContext db) : base(db, collectionName: "libraries") {}

    /*public IReadOnlyList<LibrariesWithBooksCount> GetLibrariesWithBooksCount()
    {
        var aggregation = _collection.Aggregate()
            .Lookup<Book, BsonDocument>(
                foreignCollectionName: "books",
                localField: "Guid",
                foreignField: "LibraryGuid",
                @as: "Books"
            )
            .Project(new BsonDocument
            {
                { "Guid", 1 },
                { "Name", 1 },
                { "Member", 1 },
                { "BooksCount", new BsonDocument("$size", "$Books") }
            })
            .ToList();

        var result = aggregation.Select(doc => new LibrariesWithBooksCount(
            Guid: doc["Guid"].AsGuid,
            Name: doc["Name"].AsString,
            Member: doc.Contains("Member") && doc["Member"].IsBsonNull ? null : BsonSerializer.Deserialize<User>(doc["Member"].AsBsonDocument),
            BooksCount: doc["BooksCount"].AsInt32
        )).ToList();

        return result;
    }*/
}