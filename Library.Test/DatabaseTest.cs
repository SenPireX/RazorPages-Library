using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Library.Application.Infrastructure;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Library.Test;

public class DatabaseTest
{
    protected readonly LibraryContext _db;
    private readonly MongoClient _client;
    
    public DatabaseTest()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(@"C:\Users\matea\Desktop\Coding\Library\Library.Webapp\appsettings.json")
            .Build();
        
        _client = new MongoClient(config.GetConnectionString("MongoDb"));
        _db = new LibraryContext(config);
    }
}

/*public class DatabaseTest : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly LibraryContext _db;

    public DatabaseTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var opt = new DbContextOptionsBuilder()
            .UseSqlite(_connection) // Keep connection open (only needed with SQLite in memory db)
            .Options;

        _db = new LibraryContext(opt);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}*/