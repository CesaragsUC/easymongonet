using EasyMongoNet.Utils;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace EasyMongoNet.Abstractions;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    /// <summary>
    /// Represents the MongoDB database associated with the collection.
    /// </summary>
    IMongoDatabase Database { get; }

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> to query the collection.
    /// </summary>
    IQueryable<TDocument> AsQueryable();

    /// <summary>
    /// Retrieves all documents from the collection, paginated.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of documents per page.</param>
    /// <returns>A <see cref="PagedResult{TDocument}"/> containing the paginated results.</returns>
    Task<PagedResult<TDocument>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves all documents from the collection, paginated.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of documents per page.</param>
    /// <param name="filter">The filter definition to apply.</param>
    /// <param name="_sort">The sort definition to apply.</param>
    Task<PagedResult<TDocument>> GetAllAsync(FilterDefinition<TDocument> filter, int page = 1, int pageSize = 10, string _sort = "");

    /// <summary>
    /// Filters the documents in the collection based on the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <returns>An <see cref="IEnumerable{TDocument}"/> containing the filtered results.</returns>
    Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Filters and projects the documents in the collection based on the specified expressions.
    /// </summary>
    /// <typeparam name="TProjected">The type of the projected result.</typeparam>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <param name="projectionExpression">The projection expression to apply.</param>
    /// <returns>An <see cref="IEnumerable{TProjected}"/> containing the projected results.</returns>
    Task<IEnumerable<TProjected>> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);

    /// <summary>
    /// Finds a single document in the collection that matches the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <returns>The matching document, or null if none found.</returns>
    TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously finds a single document in the collection that matches the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <returns>The matching document, or null if none found.</returns>
    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Finds a document in the collection by its ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId of the document.</param>
    /// <returns>The matching document, or null if none found.</returns>
    TDocument FindById(string id);

    /// <summary>
    /// Finds a document in the collection by a specified field and its value.
    /// </summary>
    /// <param name="field">The field to filter by.</param>
    /// <param name="id">The value of the field.</param>
    /// <returns>The matching document, or null if none found.</returns>
    TDocument FindById(string field, string id);

    /// <summary>
    /// Finds a document in the collection using a filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <returns>The matching document, or null if none found.</returns>
    TDocument FindById(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously finds a document in the collection by its ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId of the document.</param>
    /// <returns>The matching document, or null if none found.</returns>
    Task<TDocument> FindByIdAsync(string id);

    /// <summary>
    /// Asynchronously finds a document in the collection by a specified field and its value.
    /// </summary>
    /// <param name="field">The field to filter by.</param>
    /// <param name="id">The value of the field.</param>
    /// <returns>The matching document, or null if none found.</returns>
    Task<TDocument> FindByIdAsync(string field, string id);

    /// <summary>
    /// Asynchronously finds a document in the collection using a filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <returns>The matching document, or null if none found.</returns>
    Task<TDocument> FindByIdAsync(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Inserts a single document into the collection.
    /// </summary>
    /// <param name="document">The document to insert.</param>
    void InsertOne(TDocument document);

    /// <summary>
    /// Asynchronously inserts a single document into the collection.
    /// </summary>
    /// <param name="document">The document to insert.</param>
    Task InsertOneAsync(TDocument document);

    /// <summary>
    /// Inserts multiple documents into the collection.
    /// </summary>
    /// <param name="documents">The collection of documents to insert.</param>
    void InsertMany(ICollection<TDocument> documents);

    /// <summary>
    /// Asynchronously inserts multiple documents into the collection.
    /// </summary>
    /// <param name="documents">The collection of documents to insert.</param>
    Task InsertManyAsync(ICollection<TDocument> documents);

    /// <summary>
    /// Replaces an existing document in the collection.
    /// </summary>
    /// <param name="document">The document to replace.</param>
    void ReplaceOne(TDocument document);

    /// <summary>
    /// Asynchronously replaces an existing document in the collection.
    /// </summary>
    /// <param name="document">The document to replace.</param>
    Task ReplaceOneAsync(TDocument document);

    /// <summary>
    /// Asynchronously updates a document in the collection.
    /// </summary>
    /// <param name="document">The document to update.</param>
    Task UpdateAsync(TDocument document);

    /// <summary>
    /// Asynchronously updates a document in the collection by a specific field.
    /// </summary>
    /// <param name="field">The field to filter by.</param>
    /// <param name="document">The document to update.</param>
    Task UpdateAsync(string field, TDocument document);

    /// <summary>
    /// Asynchronously updates a document in the collection using a filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    Task UpdateAsync<TField>(Expression<Func<TDocument, bool>> whereCondition, Expression<Func<TDocument, TField>> field, TField value);

    /// <summary>
    /// Deletes a single document that matches the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously deletes a single document that matches the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously deletes a single document by a specific field and value.
    /// </summary>
    /// <param name="field">The field to filter by.</param>
    /// <param name="id">The value of the field.</param>
    Task DeleteAsync(string field, string id);

    /// <summary>
    /// Asynchronously deletes a single document using a filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    Task DeleteAsync(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Deletes a single document in the collection by its ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId of the document to delete.</param>
    void DeleteById(string id);

    /// <summary>
    /// Asynchronously deletes a single document in the collection by its ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId of the document to delete.</param>
    Task DeleteByIdAsync(string id);

    /// <summary>
    /// Deletes multiple documents that match the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously deletes multiple documents that match the specified filter expression.
    /// </summary>
    /// <param name="filterExpression">The filter expression to apply.</param>
    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);

    /// <summary>
    /// Asynchronously upserts a document in the collection.
    /// <param name="filterExpression">The filter expression to apply.</param>
    /// <param name="TDocument">The document to upsert.</param></param>
    Task UpsertAsync(Expression<Func<TDocument, bool>> filterExpression, TDocument document);

    /// <summary>
    /// Asynchronously upserts a document in the collection.
    /// <param name="id">Document Id</param>
    /// <param name="TDocument">The document to upsert.</param></param>
    Task UpsertAsync(string id, TDocument document);
}