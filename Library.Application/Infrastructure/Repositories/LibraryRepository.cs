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
        //List<Book> Books,
        int BooksCount,
        User? Member
    );

    private readonly ILogger<Model.Library> _logger;
    
    public LibraryRepository(LibraryContext db, ILogger<Model.Library> logger) : base(db, collectionName: "libraries", logger)
    {
        _logger = logger;
    }

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
                //{ "Books", 1 },
                { "BooksCount" , new BsonDocument( "$size", "$Books") },
                { "Member", 1 }
            })
            .ToList();
        
        var result = aggregation.Select(doc =>
            {
                var guid = doc["_id"].IsGuid ? doc["_id"].AsGuid : Guid.Parse(doc["_id"].AsString);
                //var booksList = doc["Books"].AsBsonArray
                  //  .Select(bookDoc => BsonSerializer.Deserialize<Book>(bookDoc.AsBsonDocument)).ToList();
                
                return new LibrariesWithBooksCount(
                    Guid: guid,
                    Name: doc["Name"].AsString,
                    //Books: booksList,
                    BooksCount: doc["BooksCount"].AsInt32,
                    Member: doc.Contains("Member") && doc["Member"].IsBsonNull
                        ? null
                        : BsonSerializer.Deserialize<User>(doc["Member"].AsBsonDocument)
                );
            })
            .ToList();

        return result;
    }
}