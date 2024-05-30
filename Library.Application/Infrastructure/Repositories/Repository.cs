using Library.Application.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public abstract class Repository<TEntity> where TEntity : class
{
    protected readonly LibraryContext _db;
    protected readonly IMongoCollection<TEntity> _collection;
    private readonly ILogger<TEntity> _logger;

    public Repository(LibraryContext db, string collectionName, ILogger<TEntity> logger)
    {
        _db = db;
        _collection = _db.GetDatabase().GetCollection<TEntity>(collectionName);
        _logger = logger;
    }

    public IQueryable<TEntity> Set => _collection.AsQueryable();

    public TEntity FindByGuid(Guid guid)
    {
        var guidString = guid.ToString();
        _logger.LogInformation("Searching for entity with GUID: {guidString}", guidString);

        var filter = Builders<TEntity>.Filter.Eq("_id", guidString);
        var entity = _collection.Find(filter).FirstOrDefault();

        if (entity is null)
        {
            _logger.LogWarning("Entity not found for GUID: {guidString}", guidString);
        }
        else
        {
            _logger.LogInformation("Entity found: {Entity}", entity);
        }
        return entity;
    }

    public virtual (bool success, string message) Insert(TEntity entity)
    {
        try
        {
            _collection.InsertOne(entity);
            return (true, string.Empty);
        }
        catch (MongoWriteException ex)
        {
            return (false, ex.Message);
        }
    }

    public virtual (bool success, string message) Update(TEntity entity, Guid guid)
    {
        var guidString = guid.ToString();
        var filter = Builders<TEntity>.Filter.Eq("_id", guidString);
        try
        {
            var result =  _collection.ReplaceOne(filter, entity);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return (true, string.Empty);
            }
            else
            {
                return (false, $"Updating {entity} with id: {guid} failed.");
            }
        }
        catch (MongoWriteException ex)
        {
            return (false, ex.Message);
        }
    }

    public virtual (bool success, string message) Delete(Guid guid)
    {
        var guidString = guid.ToString();
        var filter = Builders<TEntity>.Filter.Eq("_id", guidString);
        var entity = FindByGuid(guid);
        if (entity is null) { return (false, $"Entity with id: {guid} not found."); }
        try
        {
            var result =  _collection.DeleteOne(filter);
            if (result.IsAcknowledged && result.DeletedCount > 0)
            {
                return (true, string.Empty);
            }
            else
            {
                return (false, $"Deleting {entity} with id: {guid} failed.");
            }
        }
        catch (MongoWriteException ex)
        {
            return (false, ex.Message);
        }
    }
}