using EasyMongoNet.Abstractions;
using EasyMongoNet.Settings;
using EasyMongoNet.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyMongoNet.Repository;

public class MongoRepository<TDocument> : IMongoRepository<TDocument>
    where TDocument : IDocument
{
    public IMongoDatabase Database => _collection.Database;

    private readonly IMongoCollection<TDocument> _collection;

    public MongoRepository(IOptions<MongoDbSettings> settings)
    {
        var database = new MongoClient(settings.Value.ConnectionString).GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TDocument>(typeof(TDocument).Name);
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
        return _collection.AsQueryable();
    }
    public virtual async Task<PagedResult<TDocument>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var builder = Builders<TDocument>.Filter;
        var filterDefinition = builder.Empty;

        var results = await _collection
            .Find(filterDefinition)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var totalItems = await _collection.CountDocumentsAsync(filterDefinition);

        return new PagedResult<TDocument>
        {
            Items = results,
            TotalCount = (int)totalItems,
            Page = page,
            PageSize = pageSize

        };
    }

    public virtual async Task<IEnumerable<TDocument>> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _collection.Find(filterExpression).ToListAsync();
    }

    public virtual async Task<IEnumerable<TProjected>> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression)
    {
        return await _collection.Find(filterExpression).Project(projectionExpression).ToListAsync();
    }

    public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefault();
    }

    public virtual async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _collection.Find(filterExpression).FirstOrDefaultAsync();
    }

    public virtual TDocument FindById(string id)
    {
        ObjectId objectId;
        ObjectId.TryParse(id, out objectId);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return _collection.Find(filter).SingleOrDefault();
    }

    public virtual TDocument FindById(string field, string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        return _collection.Find(filter).SingleOrDefault();
    }

    public virtual TDocument FindById(Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).SingleOrDefault();
    }

    public virtual async Task<TDocument> FindByIdAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return await _collection.Find(filter).SingleOrDefaultAsync();

    }
    public virtual async Task<TDocument> FindByIdAsync(string field, string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<TDocument> FindByIdAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _collection.Find(filterExpression).FirstOrDefaultAsync();
    }

    public virtual void InsertOne(TDocument document)
    {
        _collection.InsertOne(document);
    }

    public virtual async Task InsertOneAsync(TDocument document)
    {
        await _collection.InsertOneAsync(document);
    }

    public void InsertMany(ICollection<TDocument> documents)
    {
        _collection.InsertMany(documents);
    }


    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
        await _collection.InsertManyAsync(documents);
    }

    public void ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.FindOneAndReplaceAsync(filter, document);
    }

    public virtual async Task UpdateAsync(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);

        // Define the updates needed for each <field,value>.
        var update = GetObjectProperties(document);

        await _collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        var filter = Builders<TDocument>.Filter.Where(filterExpression);

        var document = await _collection.Find(filterExpression).FirstOrDefaultAsync();

        // Define the updates needed for each <field,value>.
        var update = GetObjectProperties(document);

        await _collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateAsync(string field, TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, GetEntityId(field, document));

        var update = GetObjectProperties(document);

        await _collection.UpdateOneAsync(filter, update);
    }

    public virtual async Task UpdateAsync<TField>(Expression<Func<TDocument, bool>> whereCondition, Expression<Func<TDocument, TField>> field, TField value)
    {
        var update = Builders<TDocument>.Update.Set(field, value);
        await _collection.UpdateOneAsync(whereCondition, update);
    }


    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public async Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        await _collection.FindOneAndDeleteAsync(filterExpression);
    }

    public virtual async Task DeleteAsync(string field, string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(field, id);
        await _collection.DeleteOneAsync(filter);
    }

    public virtual async Task DeleteAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        var filter = Builders<TDocument>.Filter.Where(filterExpression);
        await _collection.DeleteOneAsync(filter);
    }

    public virtual void DeleteById(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        _collection.FindOneAndDelete(filter);
    }

    public virtual async Task DeleteByIdAsync(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        await _collection.FindOneAndDeleteAsync(filter);
    }

    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public async Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        await _collection.DeleteManyAsync(filterExpression);
    }


    private object GetEntityId(string field, TDocument entity)
    {
        var propertyInfo = entity.GetType().GetProperty(field);
        var value = propertyInfo?.GetValue(entity);

        // Check if the value is a Guid or a string
        if (value is Guid guidValue)
        {
            return guidValue;
        }

        if (value is string stringValue && Guid.TryParse(stringValue, out var parsedGuid))
        {
            return parsedGuid;
        }

        // If the value is neither Guid nor string, return as is (avoid conversion error)
        return value!;
    }

    private UpdateDefinition<T> GetObjectProperties<T>(T obj)
    {
        // Initialize the updateDefinition
        var updateDefinition = Builders<T>.Update.Combine();

        // Get the type of the object
        Type tipo = obj.GetType();

        // Loop through all the properties of the object
        foreach (PropertyInfo propriedade in tipo.GetProperties())
        {
            // Get the property name
            string nomePropriedade = propriedade.Name;

            // Get the property value
            object valorPropriedade = propriedade.GetValue(obj);

            // Add the value to the updateDefinition (If the value is not null)
            if (valorPropriedade != null)
            {
                updateDefinition = updateDefinition.Set(nomePropriedade, valorPropriedade);
            }
        }

        return updateDefinition;
    }
}