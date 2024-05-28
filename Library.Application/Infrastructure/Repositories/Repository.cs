using Library.Application.Model;
using MongoDB.Driver;

namespace Library.Application.Infrastructure.Repositories;

public abstract class Repository<TEntity> where TEntity : class
{
    protected readonly LibraryContext _db;
    protected readonly IMongoCollection<TEntity> _collection;

    public Repository(LibraryContext db, string collectionName)
    {
        _db = db;
        _collection = _db.GetDatabase().GetCollection<TEntity>(collectionName);
    }

    public IQueryable<TEntity> Set => _collection.AsQueryable();

    public virtual async Task<TEntity> FindByGuidAsync(Guid guid)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", guid);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<(bool success, string message)> InsertAsync(TEntity entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);
            return (true, string.Empty);
        }
        catch (MongoWriteException ex)
        {
            return (false, ex.Message);
        }
    }

    public virtual async Task<(bool success, string message)> UpdateAsync(TEntity entity, Guid guid)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", guid);
        try
        {
            var result = await _collection.ReplaceOneAsync(filter, entity);
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

    public virtual async Task<(bool success, string message)> DeleteAsync(Guid guid)
    {
        var filter = Builders<TEntity>.Filter.Eq("_id", guid);
        var entity = await FindByGuidAsync(guid);
        if (entity is null) { return (false, $"Entity with id: {guid} not found."); }
        try
        {
            var result = await _collection.DeleteOneAsync(filter);
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