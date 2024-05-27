using Library.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace Library.Test;

public class LibraryContextTest : DatabaseTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LibraryContextTest(ITestOutputHelper testOutputHelper) { _testOutputHelper = testOutputHelper; }

    [Fact]
    public void EnsureCreatedConnectionSuccessTest()
    {
        var client = _db.GetClient();
        //_testOutputHelper.WriteLine("Counted Collections: " + client.GetDatabase("library").ListCollections().ToList().Count);
        
        /*client.GetDatabase("library").DropCollection("books");
        client.GetDatabase("library").DropCollection("librarians");
        client.GetDatabase("library").DropCollection("libraries");
        client.GetDatabase("library").DropCollection("loans");
        client.GetDatabase("library").DropCollection("members");
        client.GetDatabase("library").DropCollection("users");
        _testOutputHelper.WriteLine("Counted Collections: " + client.GetDatabase("library").ListCollections().ToList().Count);*/
        
        /*client.GetDatabase("library").CreateCollection("books");
        client.GetDatabase("library").CreateCollection("librarians");
        client.GetDatabase("library").CreateCollection("libraries");
        client.GetDatabase("library").CreateCollection("loans");
        client.GetDatabase("library").CreateCollection("members");
        client.GetDatabase("library").CreateCollection("users");
        _testOutputHelper.WriteLine("Counted Collections: " + client.GetDatabase("library").ListCollections().ToList().Count);*/
        
        var databaseNames = client.ListDatabaseNames().ToList();
        Assert.Contains("library", databaseNames);
    }

    [Fact]
    public void SeedSuccessTest()
    {
        _db.Seed(new CryptService());
        
        Assert.True(_db.Libraries.AsQueryable().Any());
        _testOutputHelper.WriteLine($"Generated {_db.Libraries.EstimatedDocumentCount()} libraries");
        
        Assert.True(_db.Books.AsQueryable().Any());
        _testOutputHelper.WriteLine($"Generated {_db.Books.EstimatedDocumentCount()} books");
        
        Assert.True(_db.Members.AsQueryable().Any());
        _testOutputHelper.WriteLine($"Generated {_db.Members.EstimatedDocumentCount()} members");
        
        Assert.True(_db.Librarians.AsQueryable().Any());
        _testOutputHelper.WriteLine($"Generated {_db.Librarians.EstimatedDocumentCount()} librarians");
        
        Assert.True(_db.Loans.AsQueryable().Any());
        _testOutputHelper.WriteLine($"Generated {_db.Loans.EstimatedDocumentCount()} loans");
    }
}


/*public class LibraryContextTest : DatabaseTest
{
    [Fact]
    public void EnsureCreatedSuccessTest()
    {
        _db.Database.EnsureCreated();
    }

    [Fact]
    public void SeedSuccessTest()
    {
        _db.Database.EnsureCreated();
        _db.Seed(new CryptService());

        _db.ChangeTracker.Clear();
        Assert.True(_db.Libraries.ToList().Count > 0);
        Assert.True(_db.Books.ToList().Count > 0);
        Assert.True(_db.Members.ToList().Count > 0);
        Assert.True(_db.Librarians.ToList().Count > 0);
        Assert.True(_db.Loans.ToList().Count > 0);
    }
}*/