using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public class LibraryRepository : Repository<Model.Library>
{
    public record LibrariesWithBooksCount(
        Guid Guid,
        string Name,
        int BooksCount,
        User? Manager
    );
    
    public LibraryRepository(LibraryContext db, ILogger<Model.Library> logger) : base(db, collectionName: "libraries", logger) {}

    public IReadOnlyList<LibrariesWithBooksCount> GetLibrariesWithBooksCount()
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
                { "_id", 1 },
                { "Name", 1 },
                { "BooksCount" , new BsonDocument( "$size", "$Books") },
                { "Manager", 1 }
            })
            .ToList();
        
        var result = aggregation.Select(doc =>
            {
                var guid = doc["_id"].IsGuid ? doc["_id"].AsGuid : Guid.Parse(doc["_id"].AsString);
                
                return new LibrariesWithBooksCount(
                    Guid: guid,
                    Name: doc["Name"].AsString,
                    BooksCount: doc["BooksCount"].AsInt32,
                    Manager: doc.Contains("Manager") && doc["Manager"].IsBsonNull
                        ? null
                        : BsonSerializer.Deserialize<User>(doc["Manager"].AsBsonDocument)
                );
            })
            .ToList();

        return result;
    }
}